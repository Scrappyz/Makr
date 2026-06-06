using Makr.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Infrastructure.Settings
{
    public class TemplateSetting : ITemplateSetting
    {
        public string TemplateDirectory { get; set; }
    }
}
