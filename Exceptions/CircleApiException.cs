using System;
using System.Net;

namespace CircleDeveloperControlledWalletSDK.Exceptions
{
    /// <summary>
    /// Exception thrown when Circle API returns an error response.
    /// </summary>
    public class CircleApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleApiException"/> class with a specified error message and HTTP status code.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="statusCode">The HTTP status code returned by the Circle API.</param>
        public CircleApiException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleApiException"/> class with a specified error message, HTTP status code, and inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="statusCode">The HTTP status code returned by the Circle API.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public CircleApiException(string message, HttpStatusCode statusCode, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}