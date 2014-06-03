namespace Storm.GoogleAnalytics.Reporting.Configuration
{
    public interface IGoogleAnalyticsRequestConfigurerProfileId
    {
        /// <summary>
        /// This is the profile (view) id assigned to the web property you wish to query
        /// </summary>
        /// <remarks>This is not your tracking id e.g. UA-XXXXXXX-9 and can be found within the 'Reporting View Settings' within GA</remarks>
        /// <param name="value">Profile/View Id</param>
        /// <returns></returns>
        IGoogleAnalyticsRequestConfigurerDateRange WithProfileId(string value);
    }
}