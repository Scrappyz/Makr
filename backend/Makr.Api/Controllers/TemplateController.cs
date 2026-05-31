using Makr.Application.DTOs.Requests;
using Makr.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Makr.Api.Controllers
{
    [ApiController]
    public class TemplateController : BaseController
    {
        private readonly TemplateService _templateService;

        public TemplateController(TemplateService templateService)
        {
            _templateService = templateService;
        }

        [HttpPost("init")]
        public IActionResult InitTemplate([FromBody] TemplateInitRequest req)
        {
            if (_templateService.GetDuplicateVariables(req.Variables).Count > 0)
            {
                return BadRequest("Has duplicate variables");
            }

            return Ok("Good");
        }
    }
}
