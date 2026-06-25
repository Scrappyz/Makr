using Makr.Application.DTOs.Requests;
using Makr.Application.DTOs.Responses;
using Makr.Application.Interfaces;
using Makr.Application.Mappers;
using Makr.Application.Pipeline.Interpolation;
using Makr.Application.Pipeline.PathSelection;
using Makr.Application.Services.Template;
using Makr.Domain.Enums;
using Makr.Domain.Exceptions;
using Makr.Domain.Models;
using Makr.Domain.Models.Template;
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
        public async Task<IActionResult> InitTemplate([FromBody] TemplateInitRequest req)
        {
            List<ParameterKeyValue> parameters = _templateService.TransformParameterRequest(req.Parameters);
            if (_templateService.GetDuplicateParameters(parameters).Count > 0)
            {
                return BadRequest("Has duplicate parameters");
            }

            _templateService.InitializeTemplate(req.TemplateId, parameters, true);

            return Ok(_templateSetting.TemplateDirectory);
        }

        [HttpPost("info/{id}")]
        public async Task<IActionResult> GetTemplateInfo(string id)
        {
            bool templateExists = _templateService.TemplateExists(id);

            if (!templateExists)
            {
                throw new ApiException(StatusCodes.Status404NotFound, ErrorCode.Template.NotFound, $"Template with ID `{id}` cannot be found");
            }

            TemplateConfigResponse? configResponse = _templateService.GetTemplateConfig(id).ToResponse();
            return Ok(configResponse);
        }

        [HttpPost("test")]
        public async Task<IActionResult> TestAction([FromBody] TemplateInitRequest req)
        {
            Dictionary<string, string> parameters = req.Parameters.ToDictionary(p => p.Key, p => _interpolator.ToString(p.Value));
            string str = _interpolator.Interpolate("Hello, ({name})! I am at ({place}) and is ({age}) years old.", "({", "})", parameters);
            return Ok(str);
        }
    }
}
