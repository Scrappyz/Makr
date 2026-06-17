using Makr.Domain.Helpers;
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
        public object? Value
        {
            get => _value;
            set => _value = JsonUtils.UnwrapJsonElement(value);
        }

        private object? _value;
    }
}
