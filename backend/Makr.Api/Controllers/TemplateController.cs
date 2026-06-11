using Makr.Application.DTOs.Requests;
using Makr.Application.Interfaces;
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

        public TemplateController(ITemplateService templateService, ITemplateSetting templateSetting, IPathSelector pathSelector)
        {
            _templateService = templateService;
            _templateSetting = templateSetting;
            _pathSelector = pathSelector;
        }

        [HttpPost("init")] // template/init
        public IActionResult InitTemplate([FromBody] TemplateInitRequest req)
        {
            if (_templateService.GetDuplicateParameters(req.Parameters).Count > 0)
            {
                return BadRequest("Has duplicate parameters");
            }

            return Ok(_templateSetting.TemplateDirectory);
        }

        [HttpPost("test")]
        public IActionResult TestAction([FromBody] TemplateInitRequest req)
        {
            var paths = _pathSelector.GetPaths(_templateSetting.TemplateDirectory, ["python/**"], ["python/test/**"]);
            paths = _pathSelector.SortByDepthDescending(paths, '/');
            return Ok(paths);
        }
    }
}
