using Makr.Domain.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Makr.Application.DTOs.Responses
{
    [DataContract]
    public class TemplateInfoResponse
    {
        [DataMember]
        public string TemplateId { get; set; }

        [DataMember]
        public string TemplateName { get; set; }

        [DataMember]
        public string TemplateAuthor { get; set; }

        [DataMember]
        public TemplateVariableInfo[] TemplateVariables { get; set; }
    }
}
