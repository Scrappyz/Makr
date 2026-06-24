using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }

        public ApiError Error { get; set; }

        public ApiException() { }

        public ApiException(int statusCode, ApiError apiError) : base(apiError.Message)
        {
            StatusCode = statusCode;
            Error = apiError;
        }

        public ApiException(int statusCode, string errorCode, string errorMessage) : base(errorMessage)
        {
            StatusCode = statusCode;
            Error = new ApiError
            {
                Code = errorCode,
                Message = errorMessage
            };
        }
    }
}

