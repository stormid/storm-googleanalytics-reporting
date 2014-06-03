using System;

namespace Storm.GoogleAnalytics.Reporting.Core
{
    public interface IGoogleAnalyticsErrorResponse
    {
        /// <summary>
        /// Error message returned from the Google Analytics Reporting API
        /// </summary>
        /// <remarks>https://developers.google.com/analytics/devguides/reporting/core/v3/coreErrors</remarks>
        string Message { get; }
        /// <summary>
        /// Exception returned from thre Google Analytics Reporting API
        /// </summary>
        Exception Exception { get; }
    }
}