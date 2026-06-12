using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Makr.Infrastructure.Settings
{
    public class TemplateSettingPostConfigure : IPostConfigureOptions<TemplateSetting>
    {
        private readonly IHostEnvironment _env;

        public TemplateSettingPostConfigure(IHostEnvironment env)
        {
            _env = env;
        }

        public void PostConfigure(string name, TemplateSetting settings)
        {
            // Perform any post-configuration logic here

            // To make sure the path is absolute
            settings.TemplateDirectory = ResolveAbsolutePath(settings.TemplateDirectory);
            settings.TemplateInitializationDirectory = ResolveAbsolutePath(settings.TemplateInitializationDirectory);
        }

        private string ResolveAbsolutePath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            else
            {
                return Path.GetFullPath(Path.Combine(_env.ContentRootPath, path));
            }
        }
    }
}
