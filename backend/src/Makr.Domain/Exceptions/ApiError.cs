using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Exceptions
{
    public class ApiError
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public Dictionary<string, List<string>> ValidationErrors { get; set; }
    }
}
