using Makr.Domain.Models.Template;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Makr.Application.DTOs.Responses
{
    public class TemplateConfigResponse
    {
        public TemplateMetadataResponse Metadata { get; set; }

        public List<TemplateParameterResponse> Parameters { get; set; } = new List<TemplateParameterResponse>();
    }
}
