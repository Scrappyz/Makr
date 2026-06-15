namespace Makr.Domain.Models.Template
{
    public class TemplateParameter
    {
        public required string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string InputType { get; set; }

        public object? DefaultValue { get; set; }

        public bool Required { get; set; } = false;

        public bool Interpolate { get; set; }
    }
}
