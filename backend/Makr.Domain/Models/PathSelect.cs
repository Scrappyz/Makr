using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Models
{
    public class PathSelect
    {
        public List<string> include { get; set; } = new List<string>();

        public List<string> exclude { get; set; } = new List<string>();
    }
}
