namespace Makr.Domain.Models.Template
{
    public class Interpolation
    {
        public string Prefix { get; set; }

        public string Suffix { get; set; }

        public InterpolationTarget Targets { get; set; }
    }
}
