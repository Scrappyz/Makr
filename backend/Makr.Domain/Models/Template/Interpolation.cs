namespace Makr.Domain.Models.Template
{
    public class Interpolation
    {
        public string prefix { get; set; }

        public string suffix { get; set; }

        public InterpolationTarget targets { get; set; }
    }
}
