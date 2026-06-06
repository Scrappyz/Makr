using Makr.Application.DTOs.Requests;
using Makr.Application.Interfaces;
using Makr.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Makr.Api.Controllers
{
    [ApiController]
    public class TemplateController : BaseController
    {
        private readonly TemplateService _templateService;

        private readonly ITemplateSetting _templateSetting;

        public TemplateController(TemplateService templateService, ITemplateSetting templateSetting)
        {
            _templateService = templateService;
            _templateSetting = templateSetting;
        }

        [HttpPost("init")]
        public IActionResult InitTemplate([FromBody] TemplateInitRequest req)
        {
            if (_templateService.GetDuplicateParameters(req.Parameters).Count > 0)
            {
                return BadRequest("Has duplicate parameters");
            }

            return Ok(_templateSetting.TemplateDirectory);
        }
    }
}
