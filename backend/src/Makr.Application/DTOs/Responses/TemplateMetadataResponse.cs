using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Application.DTOs.Responses
{
    public class TemplateMetadataResponse
    {
        public string Name { get; set; }

        public string Author { get; set; }

        public List<string> Description { get; set; } = new List<string>();
    }
}
