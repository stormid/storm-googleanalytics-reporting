using System;
using System.IO;
using Storm.GoogleAnalytics.Reporting.Configuration;

namespace Storm.GoogleAnalytics.Reporting.Core.Impl
{
    public sealed class GoogleAnalyticsResponse : IGoogleAnalyticsResponse
    {
        public bool Success { get; private set; }
        public IGoogleAnalyticsDataResponse Data { get; private set; }
        public IGoogleAnalyticsErrorResponse Error { get; set; }

        private readonly IGoogleAnalyticsRequestConfigurationExporter _requestConfigExporter;

        public void ExportQueryTo(TextWriter writer)
        {
            if (_requestConfigExporter != null)
            {
                _requestConfigExporter.ExportTo(writer);
            }
            else
            {
                throw new ArgumentNullException("writer");
            }
        }

        internal GoogleAnalyticsResponse(IGoogleAnalyticsRequestConfiguration requestConfig, bool isSuccess, IGoogleAnalyticsDataResponse dataResponse = null,
            IGoogleAnalyticsErrorResponse errorResponse = null)
        {
            Success = isSuccess;
            Data = dataResponse;
            Error = errorResponse;
            _requestConfigExporter = requestConfig as IGoogleAnalyticsRequestConfigurationExporter;
        }
    }
}