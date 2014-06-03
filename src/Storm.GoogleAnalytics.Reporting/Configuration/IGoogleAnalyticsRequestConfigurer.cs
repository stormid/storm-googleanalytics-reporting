using System;
using System.IO;
using Storm.GoogleAnalytics.Reporting.Core;

namespace Storm.GoogleAnalytics.Reporting.Configuration
{
    public interface IGoogleAnalyticsRequestConfigurationExporter
    {
        /// <summary>
        /// Allows your request to be exported as a json object
        /// </summary>
        /// <param name="writer"></param>
        void ExportTo(TextWriter writer);
    }

    public interface IGoogleAnalyticsRequestConfigurer : IGoogleAnalyticsRequestConfigurerMetrics
    {
        /// <summary>
        /// The dimensions to query
        /// </summary>
        /// <param name="dimensions">see <see cref="GaMetadata.Dimensions"/> for standard dimension list</param>
        /// <returns></returns>
        IGoogleAnalyticsRequestConfigurer WithDimensions(params string[] dimensions);
        
        /// <summary>
        /// Provides a filter to the query
        /// </summary>
        /// <param name="field">see <see cref="GaMetadata"/></param>
        /// <param name="operator">see <see cref="GaMetadata.FilterOperator"/> for available operators</param>
        /// <param name="value">filter value</param>
        /// <returns></returns>
        IGoogleAnalyticsRequestCompositeFilterConfigurer FilterBy(string field, string @operator, string value);
        
        /// <summary>
        /// Provides a sort field and direction to the query
        /// </summary>
        /// <param name="field">see <see cref="GaMetadata.Dimensions"/> for standard dimension list</param>
        /// <param name="isDescending">sort direction</param>
        /// <returns></returns>
        IGoogleAnalyticsRequestConfigurer SortBy(string field, bool isDescending = false);

        /// <summary>
        /// Exposes the underlying request configuration
        /// </summary>
        /// <param name="custom"></param>
        /// <returns></returns>
        IGoogleAnalyticsRequestConfigurer Custom(Action<IGoogleAnalyticsRequestCustomConfigurer> custom);
    }
}