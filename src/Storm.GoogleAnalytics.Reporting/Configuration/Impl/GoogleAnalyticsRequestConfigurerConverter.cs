using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Storm.GoogleAnalytics.Reporting.Configuration.Impl
{
    internal class GoogleAnalyticsRequestConfigurerConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            serializer.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
            serializer.Formatting = Formatting.Indented;
            serializer.NullValueHandling = NullValueHandling.Ignore;
            writer.WriteStartObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var o = JObject.Load(reader);
            var profileId = o.Value<string>("ProfileId");
            var startDate = o.Value<DateTime>("StartDate");
            var endDate = o.Value<DateTime>("EndDate");

            var metrics = o["Metrics"].ToObject<string[]>().ToArray();
            var dimensions = o["Dimensions"].ToObject<string[]>().ToArray();

            var filter = o.Value<string>("Filter");
            var sort = o.Value<string>("Sort");
            var segment = o.Value<string>("Segment");
            var maxResults = o.Value<int>("MaxResults");

            var r = new GoogleAnalyticsRequestConfigurer();
            r
                .WithProfileId(profileId)
                .ForDateRange(startDate, endDate)
                .WithMetrics(metrics)
                .WithDimensions(dimensions)
                .Custom(c => c
                    .Filter(filter)
                    .Sort(sort)
                    .Segment(segment)
                    .MaxResults(maxResults)
                );

            return r.Build();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(GoogleAnalyticsRequestConfigurer) == objectType;
        }
    }
}