using System;
using System.Data;
using System.Globalization;
using System.Linq;
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
using System.Security.Cryptography.X509Certificates;

namespace Storm.GoogleAnalytics.Reporting.Impl
{
    public sealed class GoogleAnalyticsService : IGoogleAnalyticsService
    {
        public static IGoogleAnalyticsService Create(string serviceAccountId, X509Certificate2 certificate)
        {
            return new GoogleAnalyticsService(c => c
                .WithServiceAccountId(serviceAccountId)
                .WithServiceAccountCertificate(certificate));
        }

        public static IGoogleAnalyticsService Create(string serviceAccountId, string certificateAbsolutePath, string certificatePassword = "notasecret")
        {
            return new GoogleAnalyticsService(c => c
                .WithServiceAccountId(serviceAccountId)
                .WithServiceAccountCertificate(certificateAbsolutePath, certificatePassword));
        }

        private readonly IGoogleAnalyticsServiceConfiguration _serviceConfiguration;

        public GoogleAnalyticsService(
            Func<IGoogleAnalyticsServiceConfigurer, IGoogleAnalyticsServiceConfigurer> configurer)
        {
            var config = new GoogleAnalyticsServiceConfigurer();
            configurer(config);
            _serviceConfiguration = config.Build();
        }


        public IGoogleAnalyticsResponse Query(
            Func<IGoogleAnalyticsRequestConfigurerProfileId, IGoogleAnalyticsRequestConfigurerProfileId> configurer)
        {
            var config = new GoogleAnalyticsRequestConfigurer();
            configurer(config);
            return Query(config.Build());
        }

        public IGoogleAnalyticsResponse Query(IGoogleAnalyticsRequestConfiguration requestConfig)
        {
            return Task.Run(() => QueryAsync(requestConfig)).Result;
        }

        #region async methods
        public async Task<IGoogleAnalyticsResponse> QueryAsync(
            Func<IGoogleAnalyticsRequestConfigurerProfileId, IGoogleAnalyticsRequestConfigurerProfileId> configurer)
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

                    while (data.NextLink != null && data.Rows != null)
                    {
                        if (requestConfig.MaxResults < 10000 && data.Rows.Count <= requestConfig.MaxResults)
                        {
                            break;
                        }
                        gRequest.StartIndex = (gRequest.StartIndex ?? 1) + data.Rows.Count;
                        data = await gRequest.ExecuteAsync();
                        dt.Merge(ToDataTable(data));
                    }
                    return new GoogleAnalyticsResponse(requestConfig,
                        true,
                        new GoogleAnalyticsDataResponse(dt, data.ContainsSampledData.GetValueOrDefault(false)));
                }
                catch (Exception ex)
                {
                    return new GoogleAnalyticsResponse(requestConfig,
                        false, errorResponse: new GoogleAnalyticsErrorResponse(ex.Message, ex));
                }
            }
        }
        #endregion

        #region Private
        private AnalyticsService AnalyticsService
        {
            get
            {
                return new AnalyticsService(new BaseClientService.Initializer
                {
                    ApplicationName =
                        string.Format("{0}{1}", _serviceConfiguration.ApplicationName,
                            _serviceConfiguration.GZipEnabled ? " (gzip)" : ""),
                    GZipEnabled = _serviceConfiguration.GZipEnabled,
                    DefaultExponentialBackOffPolicy = ExponentialBackOffPolicy.Exception,
                    HttpClientInitializer = new ServiceAccountCredential(
                        new ServiceAccountCredential.Initializer(_serviceConfiguration.ServiceAccountId)
                        {
                            Scopes = new[] {_serviceConfiguration.Scope}
                        }.FromCertificate(_serviceConfiguration.ServiceAccountCertificate)
                        )
                });
            }
        }

        private DataResource.GaResource.GetRequest AnalyticsRequest(AnalyticsService service, IGoogleAnalyticsRequestConfiguration requestConfig)
        {
            var metrics = string.Join(",", requestConfig.Metrics.Select(GaMetadata.WithPrefix));
            var dimensions = string.Join(",", requestConfig.Dimensions.Select(GaMetadata.WithPrefix));

            var gRequest = service.Data.Ga.Get(
                GaMetadata.WithPrefix(requestConfig.ProfileId),
                requestConfig.StartDate.ToString("yyyy-MM-dd"),
                requestConfig.EndDate.ToString("yyyy-MM-dd"),
                metrics);

            gRequest.Dimensions = dimensions;
            gRequest.MaxResults = requestConfig.MaxResults;
            gRequest.Filters = requestConfig.Filter;
            gRequest.Sort = requestConfig.Sort;
            gRequest.Segment = requestConfig.Segment;

            return gRequest;
        }

        private static Type GetDataType(GaData.ColumnHeadersData gaColumn)
        {
            switch (gaColumn.DataType.ToLowerInvariant())
            {
                case "integer":
                    return typeof(int);
                case "double":
                    return typeof(double);
                case "currency":
                    return typeof (decimal);
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

        private DataTable ToDataTable(GaData response, string name = "GA")
        {
            var requestResultTable = new DataTable(name);
            if (response != null)
            {
                requestResultTable.Columns.AddRange(response.ColumnHeaders.Select(c => new DataColumn(GaMetadata.RemovePrefix(c.Name), GetDataType(c))).ToArray());

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
                                dtRow.SetField(col,
                                    DateTime.ParseExact(row[idx], "yyyyMMdd", new DateTimeFormatInfo(),
                                        DateTimeStyles.AssumeLocal));
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
        #endregion


    }
}