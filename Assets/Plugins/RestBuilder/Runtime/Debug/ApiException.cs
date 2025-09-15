using System;

namespace RestBuilder
{
    public sealed class ApiException : Exception
    {
        public long StatusCode { get; }
        public string Body { get; }

        public ApiException(long statusCode, string body)
            : base($"HTTP {statusCode}: {body}")
        {
            StatusCode = statusCode;
            Body = body;
        }
    }
}