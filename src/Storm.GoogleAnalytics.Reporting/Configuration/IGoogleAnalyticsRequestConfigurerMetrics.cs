using Storm.GoogleAnalytics.Reporting.Core;

namespace Storm.GoogleAnalytics.Reporting.Configuration
{
    public interface IGoogleAnalyticsRequestConfigurerMetrics : IGoogleAnalyticsRequestConfigurerDateRange
    {
        /// <summary>
        /// Defines the metrics to include in the query
        /// </summary>
        /// <param name="metrics">use <see cref="GaMetadata.Metrics"/> for standard list of metrics
        /// <returns></returns>
        IGoogleAnalyticsRequestConfigurer WithMetrics(params string[] metrics);
    }
}