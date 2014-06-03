using System;

namespace Storm.GoogleAnalytics.Reporting.Configuration
{
    public interface IGoogleAnalyticsRequestConfigurerDateRange : IGoogleAnalyticsRequestConfigurerProfileId
    {
        /// <summary>
        /// Specify the inclusive date range for the query
        /// </summary>
        /// <param name="startDate">Inclusive start date</param>
        /// <param name="endDate">Inclusive end date, defaults to current date if omitted</param>
        /// <returns></returns>
        IGoogleAnalyticsRequestConfigurerMetrics ForDateRange(DateTime startDate, DateTime? endDate = null);
    }
}