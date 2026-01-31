using System;
using System.Collections.Generic;

namespace VeilMail.Exceptions
{
    /// <summary>
    /// Base exception for all Veil Mail API errors.
    /// </summary>
    public class VeilMailException : Exception
    {
        public string? ErrorCode { get; }
        public int? StatusCode { get; }
        public Dictionary<string, object>? Details { get; }

        public VeilMailException(string message)
            : this(message, null, null, null) { }

        public VeilMailException(string message, string? errorCode, int? statusCode, Dictionary<string, object>? details)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
            Details = details;
        }
    }

    /// <summary>Thrown when the API key is invalid or missing (401).</summary>
    public class AuthenticationException : VeilMailException
    {
        public AuthenticationException(string message = "Invalid API key", string? errorCode = null, Dictionary<string, object>? details = null)
            : base(message, errorCode, 401, details) { }
    }

    /// <summary>Thrown when access is denied (403).</summary>
    public class ForbiddenException : VeilMailException
    {
        public ForbiddenException(string message = "Access denied", string? errorCode = null, Dictionary<string, object>? details = null)
            : base(message, errorCode, 403, details) { }
    }

    /// <summary>Thrown when the resource is not found (404).</summary>
    public class NotFoundException : VeilMailException
    {
        public NotFoundException(string message = "Resource not found", string? errorCode = null, Dictionary<string, object>? details = null)
            : base(message, errorCode, 404, details) { }
    }

    /// <summary>Thrown when request validation fails (400).</summary>
    public class ValidationException : VeilMailException
    {
        public ValidationException(string message = "Validation failed", string? errorCode = null, Dictionary<string, object>? details = null)
            : base(message, errorCode, 400, details) { }
    }

    /// <summary>Thrown when PII is detected in email content (422).</summary>
    public class PiiDetectedException : VeilMailException
    {
        public List<string> PiiTypes { get; }

        public PiiDetectedException(string message = "PII detected", List<string>? piiTypes = null, string? errorCode = null, Dictionary<string, object>? details = null)
            : base(message, errorCode, 422, details)
        {
            PiiTypes = piiTypes ?? new List<string>();
        }
    }

    /// <summary>Thrown when the rate limit is exceeded (429).</summary>
    public class RateLimitException : VeilMailException
    {
        public int? RetryAfter { get; }

        public RateLimitException(string message = "Rate limit exceeded", int? retryAfter = null, string? errorCode = null, Dictionary<string, object>? details = null)
            : base(message, errorCode, 429, details)
        {
            RetryAfter = retryAfter;
        }
    }

    /// <summary>Thrown for server errors (5xx).</summary>
    public class ServerException : VeilMailException
    {
        public ServerException(string message = "Server error", string? errorCode = null, int? statusCode = 500, Dictionary<string, object>? details = null)
            : base(message, errorCode, statusCode, details) { }
    }
}
