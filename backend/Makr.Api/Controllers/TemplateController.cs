using Makr.Application.DTOs.Requests;
using Makr.Application.Interfaces;
using Makr.Application.Pipeline.Interpolator;
using Makr.Application.Pipeline.PathSelector;
using Makr.Application.Services.Template;
using Microsoft.AspNetCore.Mvc;

namespace Makr.Api.Controllers
{
    [ApiController]
    public class TemplateController : BaseController
    {
        private readonly ITemplateService _templateService;

        private readonly ITemplateSetting _templateSetting;

        private readonly IPathSelector _pathSelector;

        private readonly IInterpolator _interpolator;

        public TemplateController(ITemplateService templateService, ITemplateSetting templateSetting, IPathSelector pathSelector, IInterpolator interpolator)
        {
            _templateService = templateService;
            _templateSetting = templateSetting;
            _pathSelector = pathSelector;
            _interpolator = interpolator;
        }

        [HttpPost("init")] // template/init
        public IActionResult InitTemplate([FromBody] TemplateInitRequest req)
        {
            if (_templateService.GetDuplicateParameters(req.Parameters).Count > 0)
            {
                return BadRequest("Has duplicate parameters");
            }

            _templateService.InitializeTemplate(req.TemplateId, req.Parameters.ToDictionary(p => p.Key, p => _interpolator.ToString(p.Value)));

            return Ok(_templateSetting.TemplateDirectory);
        }

        [HttpPost("test")]
        public IActionResult TestAction([FromBody] TemplateInitRequest req)
        {
            Dictionary<string, string> parameters = req.Parameters.ToDictionary(p => p.Key, p => _interpolator.ToString(p.Value));
            string str = _interpolator.Interpolate("Hello, ({name})! I am at ({place}) and is ({age}) years old.", "({", "})", parameters);
            return Ok(str);
        }
    }
}
