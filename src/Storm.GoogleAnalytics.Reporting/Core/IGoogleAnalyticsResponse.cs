using System.IO;

namespace Storm.GoogleAnalytics.Reporting.Core
{
    public interface IGoogleAnalyticsResponse
    {
        /// <summary>
        /// Allows you to export the request that generated this response to be exported as a json object
        /// </summary>
        /// <param name="writer"></param>
        void ExportQueryTo(TextWriter writer);
        /// <summary>
        /// Indicates whether the query executed successfully, if true then the <paramref name="Data"/> property will be populated, else see the <paramref name="Error"/> property
        /// </summary>
        bool Success { get; }
        /// <summary>
        /// Contains the success response for the executed request
        /// </summary>
        IGoogleAnalyticsDataResponse Data { get; }
        /// <summary>
        /// Contains the error response for the executed request
        /// </summary>
        IGoogleAnalyticsErrorResponse Error { get; set; }
    }
}