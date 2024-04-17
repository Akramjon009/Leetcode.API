namespace Leetcode.API.Services;

public interface ICompilerService
{
    byte[] Compile(string sourceCode);
    Task<string> ExecuteAsync(byte[] compiledAssembly);
}