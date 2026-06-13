namespace Makr.Domain.Models.Template
{
    public class PathSelect
    {
        public List<string> include { get; set; } = new List<string>();

        public List<string> exclude { get; set; } = new List<string>();
    }
}
