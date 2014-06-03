namespace Storm.GoogleAnalytics.Reporting.Configuration
{
    public interface IGoogleAnalyticsRequestCustomConfigurer
    {
        IGoogleAnalyticsRequestCustomConfigurer Segment(string value);
        IGoogleAnalyticsRequestCustomConfigurer Filter(string value);
        IGoogleAnalyticsRequestCustomConfigurer Sort(string value);
        IGoogleAnalyticsRequestCustomConfigurer MaxResults(int value = 1000);
    }
}