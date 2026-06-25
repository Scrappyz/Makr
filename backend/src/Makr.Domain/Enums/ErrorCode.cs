using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Enums
{
    public class ErrorCode
    {
        public static class General
        {
            public const string Unknown = "UNKNOWN";
            public const string ValidationFailed = "VALIDATION_FAILED";
        }

        public static class Template
        {
            public const string NotFound = "TEMPLATE_NOT_FOUND";
        }
    }
}
