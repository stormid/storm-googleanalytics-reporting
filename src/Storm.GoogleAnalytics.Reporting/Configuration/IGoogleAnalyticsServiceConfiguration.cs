using System.Security.Cryptography.X509Certificates;

namespace Storm.GoogleAnalytics.Reporting.Configuration
{
    public interface IGoogleAnalyticsServiceConfiguration
    {
        string ServiceAccountId { get; }
        X509Certificate2 ServiceAccountCertificate { get; }
        string Scope { get; }
        bool GZipEnabled { get; }
        string ApplicationName { get; }
    }
}