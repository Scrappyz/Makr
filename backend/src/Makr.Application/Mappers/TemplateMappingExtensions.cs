using Makr.Application.DTOs.Responses;
using Makr.Domain.Models.Template;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Application.Mappers
{
    public static class TemplateMappingExtensions
    {
        public static TemplateConfigResponse? ToResponse(this TemplateConfig? config)
        {
            if (config == null)
            {
                return null;
            }

            TemplateMetadataResponse metadata = new TemplateMetadataResponse
            {
                Name = config.Metadata.Name,
                Author = config.Metadata.Author,
                Description = config.Metadata.Description
            };

            List<TemplateParameterResponse> parameters = config.Initialization.Parameters
                            .Select(p => new TemplateParameterResponse
                            {
                                Key = p.Key,
                                Name = p.Name,
                                Description = p.Description,
                                InputType = p.InputType,
                                DefaultValue = p.DefaultValue,
                                Required = p.Required,
                                Interpolate = p.Interpolate
                            }).ToList();

            return new TemplateConfigResponse
            {
                Metadata = metadata,
                Parameters = parameters
            };
        }
    }
}
