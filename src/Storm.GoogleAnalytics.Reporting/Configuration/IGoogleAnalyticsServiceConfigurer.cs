using System.Security.Cryptography.X509Certificates;

namespace Storm.GoogleAnalytics.Reporting.Configuration
{
    public interface IGoogleAnalyticsServiceConfigurer
    {
        /// <summary>
        /// Specifies the service account id (usually an email address) used to access the API and query a profile
        /// </summary>
        /// <remarks>For non-interactive access you will be required to create a service account within your Google console</remarks>
        /// <param name="value">A google account created via the developer console usually ending in @developer.gserviceaccount.com</param>
        /// <param name="certificate">private key</param>
        /// <returns></returns>
        IGoogleAnalyticsServiceConfigurer WithServiceAccount(string value, X509Certificate2 certificate);
        /// <summary>
        /// Specifies the service account id (usually an email address) used to access the API and query a profile
        /// </summary>
        /// <remarks>For non-interactive access you will be required to create a service account within your Google console</remarks>
        /// <param name="value">A google account created via the developer console usually ending in @developer.gserviceaccount.com</param>
        /// <returns></returns>
        IGoogleAnalyticsServiceConfigurer WithServiceAccountId(string value);
        /// <summary>
        /// Specifies the private certificate associated with the service account id used as authorisation against the Google Analytics API
        /// </summary>
        /// <param name="certificate">private key</param>
        /// <returns></returns>
        IGoogleAnalyticsServiceConfigurer WithServiceAccountCertificate(X509Certificate2 certificate);
        /// <summary>
        /// Specifies the private certificate associated with the service account id used as authorisation against the Google Analytics API
        /// </summary>
        /// <param name="keyFile">path to key file</param>
        /// <param name="password">password for key file, those defined via the developer console will be set as 'notasecret'</param>
        /// <returns></returns>
        IGoogleAnalyticsServiceConfigurer WithServiceAccountCertificate(string keyFile, string password = "notasecret");
        /// <summary>
        /// Specifies the private certificate associated with the service account id used as authorisation against the Google Analytics API
        /// </summary>
        /// <param name="keyFile">key file as a byte array</param>
        /// <param name="password">password for key file, those defined via the developer console will be set as 'notasecret'</param>
        /// <returns></returns>
        IGoogleAnalyticsServiceConfigurer WithServiceAccountCertificate(byte[] keyFile, string password = "notasecret");
        /// <summary>
        /// Defines the scope to request when authenticating against the Google Analytics API, the default scope requests readonly access to analytics
        /// </summary>
        /// <param name="value">A complete scope string, as defined in <see cref="Google.Apis.Analytics.v3.AnalyticsService.Scope"/></param>
        /// <returns></returns>
        IGoogleAnalyticsServiceConfigurer WithScope(string value);
        /// <summary>
        /// Defines whether to use GZip compression
        /// </summary>
        /// <returns></returns>
        IGoogleAnalyticsServiceConfigurer WithGZipEnabled(bool value = true);
    }
}