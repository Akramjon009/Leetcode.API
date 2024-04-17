using Leetcode.API.DataTransferObjects;
using Leetcode.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Leetcode.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompilerController : ControllerBase
    {
        private readonly ICompilerService _compilerService;

        public CompilerController(ICompilerService compilerService)
        {
            _compilerService = compilerService;
        }

        [HttpPost]
        public async Task<IActionResult> Compile(CodeModel codeModel)
        {
            byte[] compiledCode = _compilerService.Compile(codeModel.Code);
            string result = await _compilerService.ExecuteAsync(compiledCode);

            return Ok(result);
        }
    }
}
