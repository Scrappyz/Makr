using Makr.Domain.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Makr.Application.DTOs.Requests
{
    [DataContract]
    public class TemplateInitRequest
    {
        [DataMember(IsRequired = true)]
        public string TemplateId { get; set; }

        [DataMember(IsRequired = false)]
        public List<TemplateParameterRequest> Parameters { get; set; } = new List<TemplateParameterRequest>();
    }
}
