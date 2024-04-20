using Leetcode.API.DataTransferObjects;
using Leetcode.API.Models;
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
            => _compilerService = compilerService;

        [HttpPost]
        public async Task<IActionResult> Compile(CodeModel codeModel)
        {
            try
            {
                byte[] compiledCode = _compilerService.Compile(codeModel.Code);
                string result = await _compilerService.ExecuteAsync(compiledCode);

                return Ok(new CodeViewModel()
                {
                    Result = result,
                });
            }
            catch (Exception ex)
            {
                return Ok(new CodeViewModel()
                {
                    Result = ex.Message,
                });
            }
        }
    }
}
