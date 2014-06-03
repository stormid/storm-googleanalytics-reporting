using System;

namespace Storm.GoogleAnalytics.Reporting.Core.Impl
{
    public sealed class GoogleAnalyticsErrorResponse : IGoogleAnalyticsErrorResponse
    {
        public GoogleAnalyticsErrorResponse(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }

        public string Message { get; private set; }
        public Exception Exception { get; private set; }
    }
}