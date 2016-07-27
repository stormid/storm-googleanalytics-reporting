using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Services;
using Storm.GoogleAnalytics.Reporting.Configuration;
using Storm.GoogleAnalytics.Reporting.Configuration.Impl;
using Storm.GoogleAnalytics.Reporting.Core;
using Storm.GoogleAnalytics.Reporting.Core.Impl;

namespace Storm.GoogleAnalytics.Reporting.Impl
{
    public class RealTimeGoogleAnalyticsService : IRealTimeGoogleAnalyticsService
    {
        public static IRealTimeGoogleAnalyticsService Create(string serviceAccountId, X509Certificate2 certificate)
        {
            return new RealTimeGoogleAnalyticsService(x => x
                .WithServiceAccountId(serviceAccountId)
                .WithServiceAccountCertificate(certificate));
        }

        public static IRealTimeGoogleAnalyticsService Create(string serviceAccountId, string certificateAbsolutePath, string certificatePassword = "notasecret")
        {
            return new RealTimeGoogleAnalyticsService(x => x
                .WithServiceAccountId(serviceAccountId)
                .WithServiceAccountCertificate(certificateAbsolutePath, certificatePassword));
        }

        private readonly IGoogleAnalyticsServiceConfiguration _serviceConfiguration;

        public RealTimeGoogleAnalyticsService(Func<IGoogleAnalyticsServiceConfigurer, IGoogleAnalyticsServiceConfigurer> configurer)
        {
            var config = new GoogleAnalyticsServiceConfigurer();
            configurer(config);
            _serviceConfiguration = config.Build();
        }

        public IGoogleAnalyticsResponse Query(Func<IGoogleAnalyticsRequestConfigurerProfileId, IGoogleAnalyticsRequestConfigurerProfileId> configurer)
        {
            var config = new GoogleAnalyticsRequestConfigurer();
            configurer(config);
            return Query(config.Build());
        }

        public IGoogleAnalyticsResponse Query(IGoogleAnalyticsRequestConfiguration requestConfig)
        {
            return Task.Run(() => QueryAsync(requestConfig)).Result;
        }

        public async Task<IGoogleAnalyticsResponse> QueryAsync(Func<IGoogleAnalyticsRequestConfigurerProfileId, IGoogleAnalyticsRequestConfigurerProfileId> configurer)
        {
            var config = new GoogleAnalyticsRequestConfigurer();
            configurer(config);
            return await QueryAsync(config.Build());
        }

        public async Task<IGoogleAnalyticsResponse> QueryAsync(IGoogleAnalyticsRequestConfiguration requestConfig)
        {
            using (var svc = AnalyticsService)
            {
                try
                {
                    var gRequest = AnalyticsRequest(svc, requestConfig);

                    var data = await gRequest.ExecuteAsync();
                    var dt = ToDataTable(data, data.ProfileInfo.ProfileName);

                    return new GoogleAnalyticsResponse(requestConfig, true, new GoogleAnalyticsDataResponse(dt, false));
                }
                catch (Exception ex)
                {
                    return new GoogleAnalyticsResponse(requestConfig, false, errorResponse: new GoogleAnalyticsErrorResponse(ex.Message, ex));
                }
            }
        }

        private Google.Apis.Analytics.v3.AnalyticsService AnalyticsService
        {
            get
            {
                return new Google.Apis.Analytics.v3.AnalyticsService(new BaseClientService.Initializer
                {
                    ApplicationName = string.Format("{0}{1}", _serviceConfiguration.ApplicationName, _serviceConfiguration.GZipEnabled ? " (gzip)" : ""),
                    GZipEnabled = _serviceConfiguration.GZipEnabled,
                    DefaultExponentialBackOffPolicy = ExponentialBackOffPolicy.Exception,
                    HttpClientInitializer = new ServiceAccountCredential(
                        new ServiceAccountCredential.Initializer(_serviceConfiguration.ServiceAccountId)
                        {
                            Scopes = new[] { _serviceConfiguration.Scope }
                        }.FromCertificate(_serviceConfiguration.ServiceAccountCertificate)
                        )
                });
            }
        }

        private DataResource.RealtimeResource.GetRequest AnalyticsRequest(Google.Apis.Analytics.v3.AnalyticsService service, IGoogleAnalyticsRequestConfiguration requestConfig)
        {
            var metrics = string.Join(",", requestConfig.Metrics.Select(GaMetadata.WithRealtimePrefix));
            var dimensions = string.Join(",", requestConfig.Dimensions.Select(GaMetadata.WithRealtimePrefix));

            var gRequest = service.Data.Realtime.Get(
                GaMetadata.WithPrefix(requestConfig.ProfileId),
                metrics);

            gRequest.Dimensions = dimensions == "" ? null : dimensions;
            gRequest.MaxResults = requestConfig.MaxResults;
            gRequest.Filters = requestConfig.Filter;
            gRequest.Sort = requestConfig.Sort;

            return gRequest;
        }

        private DataTable ToDataTable(RealtimeData response, string name = "GA")
        {
            var requestResultTable = new DataTable(name);
            if (response != null)
            {
                requestResultTable.Columns.AddRange(response.ColumnHeaders.Select(x => new DataColumn(GaMetadata.RemoveRealtimePrefix(x.Name), GetDataType(x))).ToArray());

                if (response.Rows != null)
                {
                    foreach (var row in response.Rows)
                    {
                        var dtRow = requestResultTable.NewRow();

                        for (var idx = 0; idx != requestResultTable.Columns.Count; idx++)
                        {
                            var col = requestResultTable.Columns[idx];
                            if (col.DataType == typeof(DateTime))
                            {
                                dtRow.SetField(col, DateTime.ParseExact(row[idx], "yyyyMMdd", new DateTimeFormatInfo(), DateTimeStyles.AssumeLocal));
                            }
                            else
                            {
                                dtRow.SetField(col, row[idx]);
                            }
                        }
                        requestResultTable.Rows.Add(dtRow);
                    }
                }
                requestResultTable.AcceptChanges();
            }
            return requestResultTable;
        }

        private static Type GetDataType(RealtimeData.ColumnHeadersData gaColumn)
        {
            switch (gaColumn.DataType.ToLowerInvariant())
            {
                case "integer":
                    return typeof(int);
                case "double":
                    return typeof(double);
                case "currency":
                    return typeof(decimal);
                case "time":
                    return typeof(float);
                default:
                    if (gaColumn.Name.ToLowerInvariant().Equals(GaMetadata.WithPrefix(GaMetadata.Dimensions.Time.Date)))
                    {
                        return typeof(DateTime);
                    }
                    return typeof(string);
            }
        }
    }
}