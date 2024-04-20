using Leetcode.Application.DataTransferObjects;
using Leetcode.Application.Services;
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

        [HttpPost("Compile")]
        public async Task<IActionResult> Compile(
            SubmissionCreationDTO submissionCreationDTO,
            CancellationToken cancellationToken)
            => Ok(await _compilerService.Compile(submissionCreationDTO, cancellationToken));
    }
}
