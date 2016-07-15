using System;
using System.Threading.Tasks;
using Storm.GoogleAnalytics.Reporting.Configuration;
using Storm.GoogleAnalytics.Reporting.Core;

namespace Storm.GoogleAnalytics.Reporting
{
    public interface IRealTimeGoogleAnalyticsService
    {
        IGoogleAnalyticsResponse Query(Func<IGoogleAnalyticsRequestConfigurerProfileId, IGoogleAnalyticsRequestConfigurerProfileId> configurer);
        IGoogleAnalyticsResponse Query(IGoogleAnalyticsRequestConfiguration requestConfig);
        Task<IGoogleAnalyticsResponse> QueryAsync(Func<IGoogleAnalyticsRequestConfigurerProfileId, IGoogleAnalyticsRequestConfigurerProfileId> configurer);
        Task<IGoogleAnalyticsResponse> QueryAsync(IGoogleAnalyticsRequestConfiguration requestConfig);
    }
}