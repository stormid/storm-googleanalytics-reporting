storm-googleanalytics-reporting
==============

An abstraction over the Google Analytics v3 Reporting API, providing a clean, fluent method of querying analytics data.

Install
==============

The library is available through NuGet...
```
install-package storm.googleanalytics.reporting
```

Usage
==============

Lets get straight to an example.

```C#
public class MyData
{
	public DateTime Date { get; set; }
	public string Country { get; set; }
	public int Pageviews { get; set; }
	public int Visits { get; set; }
}

public static void Main(string[] args)
{
	var service = GoogleAnalyticsService.Create("serviceAccountId", "path/to/certificate");

	var result = service.Query(q => q
		.WithProfileId("1111111")
		.ForDateRange(DateTime.Today.AddDays(-1), DateTime.Today)
		.WithMetrics(GaMetadata.Metrics.PageTracking.PageViews, GaMetadata.Metrics.Session.Visits)
		.WithDimensions(GaMetadata.Dimensions.Time.Date, GaMetadata.Dimensions.GeoNetwork.Country)
		.FilterBy(GaMetadata.Dimensions.PlatformDevice.DeviceCategory, GaMetadata.FilterOperator.IsEqual, "tablet")
		.OrFilterBy(GaMetadata.Dimensions.PlatformDevice.DeviceCategory, GaMetadata.FilterOperator.IsEqual, "mobile")
		.SortBy(GaMetadata.Metrics.PageTracking.PageViews, true)
		);

	if (result.Success)
	{
		Console.WriteLine(result.Data.AsJson(true));

		var dt = result.Data.AsDataTable();

		IEnumerable<MyData> entities = result.Data.ToObject<MyData>();
	}
}
```

Obtaining a Service Account and Certificate
================
In order to access the Google Analytics Reporting API you must generate a service account using the Google Developers Console (https://console.developers.google.com).  Within the console you need to create a project and generate a new "Client ID" credential with the "Service Account" application type.

Once created you will receive a certificate key file and an email address (this is your service account id).