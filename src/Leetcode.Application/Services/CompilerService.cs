using Leetcode.API.Services;
using Leetcode.Application.DataTransferObjects;
using Leetcode.Domain.Exceptions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Leetcode.Application.Services;

public class CompilerService : ICompilerService
{
    #region Compile process

    public byte[] Compile(string sourceCode)
    {
        using var peStream = new MemoryStream();

        var result = GenerateCode(sourceCode).Emit(peStream);

        if (result.Success is false)
        {
            StringBuilder message = new StringBuilder();

            var failures = result
                .Diagnostics
                .Where(diagnostic => diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

            foreach (var diagnostic in failures)
            {
                string diagnosticMessage = diagnostic
                    .GetMessage()
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;");

                string diagnosticId = diagnostic.Id;

                message.AppendLine($"{diagnosticId}: {diagnosticMessage}\n");

                message.AppendLine();
            }

            throw new Exception(message.ToString());
        }

        peStream.Seek(0, SeekOrigin.Begin);

        return peStream.ToArray();
    }

    private CSharpCompilation GenerateCode(string sourceCode)
    {
        var codeString = SourceText.From(sourceCode);

        var options = CSharpParseOptions
            .Default
            .WithLanguageVersion(LanguageVersion.CSharp11);

        var parsedSyntaxTree = SyntaxFactory
            .ParseSyntaxTree(codeString, options);

        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)
        };

        Assembly
            .GetEntryAssembly()?
            .GetReferencedAssemblies()
            .ToList()
            .ForEach(assembly => references
                .Add(MetadataReference.CreateFromFile(Assembly.Load(assembly).Location)));

        return CSharpCompilation.Create("HelloWorld.dll",
            new[] { parsedSyntaxTree },
            references: references,
            options: new CSharpCompilationOptions(
                OutputKind.ConsoleApplication,
                optimizationLevel: OptimizationLevel.Release,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
    }

    #endregion

    #region Run process
    public async ValueTask<string> ExecuteAsync(byte[] compiledAssembly)
        => await LoadAndExecuteAsync(compiledAssembly);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private async ValueTask<string> LoadAndExecuteAsync(byte[] compiledAssembly)
    {
        using var memoryStream = new MemoryStream(compiledAssembly);
        var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();
        var assembly = assemblyLoadContext.LoadFromStream(memoryStream);
        var entry = assembly.EntryPoint;
        using var outputStream = new MemoryStream();
        using var streamWriter = new StreamWriter(outputStream);
        streamWriter.AutoFlush = true;
        var originalConsoleOut = Console.Out;
        int timeoutMilliseconds = 5000;

        try
        {
            Console.SetOut(streamWriter);

            if (entry is not null)
            {
                var args = entry.GetParameters().Length > 0 ?
                    new object[] { Array.Empty<string>() } :
                    default;

                var task = Task.Run(() => entry.Invoke(null, args));

                if (await Task.WhenAny(task, Task.Delay(timeoutMilliseconds)) != task)
                {
                    throw new TimeoutException($"The assembly execution timed out after {timeoutMilliseconds} ms.");
                }

                await task;
            }
        }
        finally
        {
            Console.SetOut(originalConsoleOut);
        }

        string output = Encoding.UTF8.GetString(outputStream.ToArray());
        assemblyLoadContext.Unload();

        CollectWeakReferences(assemblyLoadContext);

        return output;
    }

    private void CollectWeakReferences(SimpleUnloadableAssemblyLoadContext assemblyLoadContext)
    {
        var assemblyLoadContextWeakRef = new WeakReference(assemblyLoadContext);

        for (var i = 0; i < 8 && assemblyLoadContextWeakRef.IsAlive; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    public async ValueTask<CompilationResult> Compile(SubmissionCreationDTO submission, CancellationToken cancellationToken)
    {
        if (submission.Code is null)
            throw new ValidationException();

        try
        {
            byte[] compiledCode = Compile(submission.Code);
            string result = await ExecuteAsync(compiledCode);

            return new CompilationResult()
            {
                Result = result,
            };
        }
        catch (Exception ex)
        {
            return new CompilationResult()
            {
                Error = ex.Message,
            };
        }
    }
    #endregion
}