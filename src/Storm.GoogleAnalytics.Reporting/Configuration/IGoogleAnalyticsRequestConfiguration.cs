using System;
using System.Collections.Generic;

namespace Storm.GoogleAnalytics.Reporting.Configuration
{
    public interface IGoogleAnalyticsRequestConfiguration
    {
        string ProfileId { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        IEnumerable<string> Metrics { get; }
        IEnumerable<string> Dimensions { get; }
        string Filter { get; }
        string Sort { get; }
        string Segment { get; }
        int MaxResults { get; }
    }
}