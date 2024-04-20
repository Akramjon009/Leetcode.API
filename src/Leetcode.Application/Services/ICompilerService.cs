using Leetcode.Application.DataTransferObjects;

namespace Leetcode.Application.Services;

public interface ICompilerService
{
    byte[] Compile(string sourceCode);
    ValueTask<string> ExecuteAsync(byte[] compiledAssembly);
    ValueTask<CompilationResult> Compile(SubmissionCreationDTO submission, CancellationToken cancellationToken);
}