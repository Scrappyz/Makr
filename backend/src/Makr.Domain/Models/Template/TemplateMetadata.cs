namespace Makr.Domain.Models.Template
{
    public class TemplateMetadata
    {
        public required string Name { get; set; }

        public string Author { get; set; }

        public List<string> Description { get; set; } = new List<string>(); // For multi-line string

        public List<string> Tags { get; set; } = new List<string>();
    }
}
