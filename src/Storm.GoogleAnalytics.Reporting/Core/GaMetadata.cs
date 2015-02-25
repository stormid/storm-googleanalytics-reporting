namespace Storm.GoogleAnalytics.Reporting.Core
{
    public static class GaMetadata
    {
        private const string Prefix = "ga:";

        public static string WithPrefix(string value)
        {
            if (value.StartsWith(Prefix)) return value;
            return string.Format("{0}{1}", Prefix, value);
        }

        public static string RemovePrefix(string value)
        {
            if (value.StartsWith(Prefix))
            {
                return value.Substring(Prefix.Length);
            }
            return value;
        }

        public static class FilterOperator
        {
            public static string IsEqual = "==";
            public static string NotEquals = "!=";
            public static string GreaterThan = ">";
            public static string LessThan = "<";
            public static string GreaterThanOrEquals = ">=";
            public static string LessThanOrEquals = "<=";
            public static string Contains = "=@";
            public static string NotContains = "!@";
            public static string MatchRegEx = "=~";
            public static string NotMatchRegEx = "!~";
            public static string Or = ",";
            public static string And = ";";
        }

        public static class Dimensions
        {
            public static class Visitor
            {
                public static string VisitorType = "visitorType";
                public static string VisitCount = "visits";
                public static string DaysSinceLastVisit = "daysSinceLastVisit";
            }

            public static class Session
            {
                public static string VisitLength = "visitLength";
            }

            public static class Time
            {
                public static string Date = "date";
                public static string Year = "year";
                public static string Month = "month";
                public static string Week = "week";
                public static string Day = "day";
                public static string Hour = "hour";
                public static string YearMonth = "yearMonth";
                public static string YearWeek = "yearWeek";
                public static string DateHour = "dateHour";
                public static string NthWeek = "nthWeek";
                public static string NthDay = "nthDay";
                public static string IsoWeek = "isoWeek";
                public static string DayOfWeek = "dayOfWeek";
                public static string DayOfWeekName = "dayOfWeekname";
            }

            public static class GeoNetwork
            {
                public static string Continent = "region";
                public static string SubContinent = "region";
                public static string Country = "country";
                public static string Region = "region";
                public static string Metro = "metro";
                public static string City = "city";
                public static string Latitude = "latitude";
                public static string Longitude = "longitude";
                public static string NetworkDomain = "networkDomain";
                public static string NetworkLocation = "networkLocation";

            }

            public static class PlatformDevice
            {
                public static string Browser = "browser";
                public static string BrowserVersion = "browserVersion";
                public static string OperatingSystem = "operatingSystem";
                public static string OperatingSystemVersion = "operatingSystemVersion";
                public static string DeviceCategory = "deviceCategory";
                public static string IsMobile = "isMobile";
                public static string IsTablet = "isTablet";
                public static string MobileDeviceBranding = "mobileDeviceBranding";
                public static string MobileDeviceMarketingName = "mobileDeviceMarketingName";
                public static string MobileDeviceModel = "mobileDeviceModel";
                public static string MobileInputSelector = "mobileInputSelector";
                public static string MobileDeviceInfo = "mobileDeviceInfo";
            }

            public static class PageTracking
            {
                public static string PagePath = "pagePath";
                public static string PageTitle = "pageTitle";
            }

            public static class CustomVariable
            {
                public static string One = "customVarValue1";
                public static string Two = "customVarValue2";
                public static string Three = "customVarValue3";
                public static string Four = "customVarValue4";
                public static string Five = "customVarValue5";
            }

            public static string CustomDimension(int index)
            {
                return string.Format("dimension{0}", index);
            }

            public static class EventTracking
            {
                public static string Category = "eventCategory";
                public static string Action = "eventAction";
                public static string Label = "eventLabel";
            }

            public static class TrafficSources
            {
                public static string ReferralPath = "referralPath";
                public static string FullReferrer = "fullReferrer";
                public static string Campaign = "campaign";
                public static string Source = "source";
                public static string Medium = "medium";
                public static string SourceMedium = "sourceMedium";
                public static string Keyword = "keyword";
                public static string AdContent = "adContent";
                public static string SocialNetwork = "socialNetwork";
                public static string HasSocialSourceReferral = "hasSocialSourceReferral";
            }
        }

        public static class Metrics
        {
            public static class Visitor
            {
                public static string Visitors = "visitors";
                public static string NewVisits = "newVisits";
            }

            public static class Session
            {
                public static string Visits = "visits";
                public static string Bounces = "bounces";
                public static string TimeOnSite = "timeOnSite";
                public static string AvgTimeOnSite = "avgTimeOnSite";
            }

            public static class PageTracking
            {
                public static string Entrances = "entrances";
                public static string PageViews = "pageviews";
                public static string UniquePageViews = "uniquepageviews";
                public static string TimeOnPage = "timeOnPage";
                public static string Exits = "exits";
                public static string AvgTimeOnPage = "avgTimeOnPage";
            }

            public static class EventTracking
            {
                public static string TotalEvents = "totalEvents";
                public static string UniqueEvents = "uniqueEvents";
                public static string EventValue = "eventValue";
                public static string AvgEventValue = "avgEventValue";
                public static string VisitsWithEvent = "visitsWithEvent";
                public static string EventsPerVisitWithEvent = "eventsPerVisitWithEvent";
            }

            public static class TrafficSources
            {
                public static string OrganicSearches = "organicSearches";
            }

            public static string Custom(int index)
            {
                return string.Format("metric{0}", index);
            }

        }
    }
}