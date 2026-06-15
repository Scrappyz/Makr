namespace Makr.Domain.Models.Template
{
    public class InterpolationTarget
    {
        public PathSelect Content { get; set; } = new PathSelect();

        public PathSelect Path { get; set; } = new PathSelect();
    }
}
