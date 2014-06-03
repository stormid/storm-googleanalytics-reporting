namespace Storm.GoogleAnalytics.Reporting.Configuration
{
    public interface IGoogleAnalyticsRequestCompositeFilterConfigurer : IGoogleAnalyticsRequestConfigurer
    {
        IGoogleAnalyticsRequestCompositeFilterConfigurer OrFilterBy(string field, string @operator, string value);
        IGoogleAnalyticsRequestCompositeFilterConfigurer AndFilterBy(string field, string @operator, string value);
    }
}