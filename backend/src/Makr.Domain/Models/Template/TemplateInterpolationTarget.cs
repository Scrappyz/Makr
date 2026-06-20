namespace Makr.Domain.Models.Template
{
    public class TemplateInterpolationTarget
    {
        public PathSelect Contents { get; set; } = new PathSelect();

        public PathSelect Filenames { get; set; } = new PathSelect();
    }
}
