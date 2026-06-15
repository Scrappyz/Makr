using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Makr.Domain.Models
{
    [DataContract]
    public class TemplateParameterRequest
    {
        [DataMember(IsRequired = true)]
        public string Key { get; set; }

        [DataMember(IsRequired = false)]
        public object? Value { get; set; }
    }
}
