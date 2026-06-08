using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Models
{
    public class TemplateMetadata
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
    }
}
