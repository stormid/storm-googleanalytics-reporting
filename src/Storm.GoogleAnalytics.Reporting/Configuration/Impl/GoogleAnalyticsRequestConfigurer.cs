using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Storm.GoogleAnalytics.Reporting.Core;

namespace Storm.GoogleAnalytics.Reporting.Configuration.Impl
{
    public class GoogleAnalyticsRequestConfigurer : IGoogleAnalyticsRequestConfiguration, IGoogleAnalyticsRequestCompositeFilterConfigurer, IGoogleAnalyticsRequestCustomConfigurer, IGoogleAnalyticsRequestConfigurationExporter
    {
        public string ProfileId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public IEnumerable<string> Metrics { get; private set; }
        public IEnumerable<string> Dimensions { get; private set; }
        public string Filter { get; private set; }
        public string Sort { get; private set; }
        public string Segment { get; private set; }
        public int MaxResults { get; private set; }

        public static GoogleAnalyticsRequestConfigurer LoadFrom(string path)
        {
            if(!File.Exists(path)) throw new FileNotFoundException("request file not found", path);

            using (var stream = File.OpenText(path))
            {
                using (var reader = new JsonTextReader(stream))
                {
                    var s = new JsonSerializer()
                    {
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore,
                    };

                    s.Converters.Add(new GoogleAnalyticsRequestConfigurerConverter());
                    return s.Deserialize<GoogleAnalyticsRequestConfigurer>(reader);
                }
            }
        }

        public void ExportTo(TextWriter writer)
        {
            if(writer == null) throw new ArgumentNullException("writer");

            using (var jsonWriter = new JsonTextWriter(writer))
            {
                var s = new JsonSerializer()
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,
                };
                s.Serialize(jsonWriter, this);
            }
        }

        public GoogleAnalyticsRequestConfigurer()
        {
            MaxResults = 1000;
            Filter = null;
            Sort = null;
            Segment = null;
            Dimensions = Enumerable.Empty<string>();
            Metrics = Enumerable.Empty<string>();
            EndDate = DateTime.Today;
        }

        public IGoogleAnalyticsRequestConfiguration Build()
        {
            return this;
        }

        public IGoogleAnalyticsRequestConfigurerDateRange WithProfileId(string value)
        {
            ProfileId = GaMetadata.RemovePrefix(value);
            return this;
        }

        public IGoogleAnalyticsRequestConfigurerMetrics ForDateRange(DateTime startDate, DateTime? endDate = null)
        {
            if(startDate > endDate.GetValueOrDefault(DateTime.Today)) throw new ArgumentOutOfRangeException("startDate", "startDate must be less than or equal to endDate");

            StartDate = startDate;
            EndDate = endDate.GetValueOrDefault(DateTime.Today);
            return this;
        }

        public IGoogleAnalyticsRequestConfigurer WithMetrics(params string[] metrics)
        {
            Metrics = metrics.Select(GaMetadata.RemovePrefix);
            return this;
        }

        public IGoogleAnalyticsRequestConfigurer WithDimensions(params string[] dimensions)
        {
            Dimensions = dimensions.Select(GaMetadata.RemovePrefix);
            return this;
        }

        IGoogleAnalyticsRequestCompositeFilterConfigurer IGoogleAnalyticsRequestCompositeFilterConfigurer.OrFilterBy(string field, string @operator, string value)
        {
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                Filter = string.Format("{0}{1}{2}", Filter, GaMetadata.FilterOperator.Or, BuildFilter(field, @operator, value));
                return this;
            }
            return FilterBy(field, @operator, value);
        }

        IGoogleAnalyticsRequestCompositeFilterConfigurer IGoogleAnalyticsRequestCompositeFilterConfigurer.AndFilterBy(string field, string @operator, string value)
        {
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                Filter = string.Format("{0}{1}{2}", Filter, GaMetadata.FilterOperator.And, BuildFilter(field, @operator, value));
                return this;
            }
            return FilterBy(field, @operator, value);
        }

        public IGoogleAnalyticsRequestCompositeFilterConfigurer FilterBy(string field, string @operator, string value)
        {
            Filter = BuildFilter(field, @operator, value);
            return this;
        }

        private static string BuildFilter(string field, string @operator, string value)
        {
            if (string.IsNullOrWhiteSpace(field)) throw new ArgumentNullException("field", "filter field must be specified, see GaMetadata");
            if (string.IsNullOrWhiteSpace(@operator)) throw new ArgumentNullException("@operator", "filter operator must be specified, see GaMetadata.FilterOperator");
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("value", "filter value must be specified");

            return string.Format("{0}{1}{2}", GaMetadata.WithPrefix(field), @operator, value);
        }

        public IGoogleAnalyticsRequestConfigurer WithCustomFilter(string filter)
        {
            Filter = filter;
            return this;
        }

        public IGoogleAnalyticsRequestConfigurer SortBy(string field, bool isDescending = false)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = BuildSort(field, isDescending);
            }
            else
            {
                Sort = string.Format("{0},{1}", Sort, BuildSort(field, isDescending));
            }
            return this;
        }

        private static string BuildSort(string field, bool descending = false)
        {
            if (string.IsNullOrWhiteSpace(field)) throw new ArgumentNullException("field", "sort field must be specified, see GaMetadata");

            return string.Format("{0}{1}", descending ? "-" : "", GaMetadata.WithPrefix(field));
        }

        #region Custom
        public IGoogleAnalyticsRequestConfigurer Custom(Action<IGoogleAnalyticsRequestCustomConfigurer> custom)
        {
            custom(this);
            return this;
        }

        IGoogleAnalyticsRequestCustomConfigurer IGoogleAnalyticsRequestCustomConfigurer.Segment(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Segment = value;
            }
            return this;
        }

        IGoogleAnalyticsRequestCustomConfigurer IGoogleAnalyticsRequestCustomConfigurer.Filter(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Filter = value;
            }
            return this;
        }

        IGoogleAnalyticsRequestCustomConfigurer IGoogleAnalyticsRequestCustomConfigurer.Sort(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Sort = value;
            }
            return this;
        }

        IGoogleAnalyticsRequestCustomConfigurer IGoogleAnalyticsRequestCustomConfigurer.MaxResults(int value)
        {
            if (value < 1 || value > 10000) throw new ArgumentOutOfRangeException("value", "max results must be between 1 and 10000");
            MaxResults = value;
            return this;
        }
        #endregion
    }
}
