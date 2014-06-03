using System;
using System.Threading.Tasks;
using Storm.GoogleAnalytics.Reporting.Configuration;
using Storm.GoogleAnalytics.Reporting.Core;

namespace Storm.GoogleAnalytics.Reporting
{
    public interface IGoogleAnalyticsService
    {
        /// <summary>
        /// Query the Google Analytics Reporting API using the given request configuration
        /// </summary>
        /// <param name="configurer">configures the request with which to query the Google Analytics Reporting API</param>
        /// <returns>Google Analytics Response</returns>
        IGoogleAnalyticsResponse Query(Func<IGoogleAnalyticsRequestConfigurerProfileId, IGoogleAnalyticsRequestConfigurerProfileId> configurer);
        /// <summary>
        /// Query the Google Analytics Reporting API using the given request configuration
        /// </summary>
        /// <param name="requestConfig">the request with which to query the Google Analytics Reporting API</param>
        /// <returns>Google Analytics Response</returns>
        IGoogleAnalyticsResponse Query(IGoogleAnalyticsRequestConfiguration requestConfig);
        /// <summary>
        /// Query the Google Analytics Reporting API using the given request configuration
        /// </summary>
        /// <param name="configurer">configures the request with which to query the Google Analytics Reporting API</param>
        /// <returns>Google Analytics Response</returns>
        Task<IGoogleAnalyticsResponse> QueryAsync(Func<IGoogleAnalyticsRequestConfigurerProfileId, IGoogleAnalyticsRequestConfigurerProfileId> configurer);
        /// <summary>
        /// Query the Google Analytics Reporting API using the given request configuration
        /// </summary>
        /// <param name="requestConfig">the request with which to query the Google Analytics Reporting API</param>
        /// <returns>Google Analytics Response</returns>
        Task<IGoogleAnalyticsResponse> QueryAsync(IGoogleAnalyticsRequestConfiguration requestConfig);
    }
}