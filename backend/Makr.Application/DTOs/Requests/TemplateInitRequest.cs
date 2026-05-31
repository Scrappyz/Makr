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
        [DataMember]
        public string TemplateId { get; set; }

        [DataMember]
        public TemplateVariable[] Variables { get; set; }
    }
}
