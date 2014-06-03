using System.Collections.Generic;
using System.Data;

namespace Storm.GoogleAnalytics.Reporting.Core
{
    public interface IGoogleAnalyticsDataResponse
    {
        /// <summary>
        /// Determines whether the response was produced using sampled data
        /// </summary>
        bool HasSampledData { get; }
        /// <summary>
        /// Outputs the query result as a structured datatable containing dimensions and metrics as columns and a row per result item
        /// </summary>
        /// <returns>A datatable containing the query result</returns>
        DataTable AsDataTable();
        /// <summary>
        /// Outputs the query result as a JSON string 
        /// </summary>
        /// <param name="prettyPrinted">should the output by printed with indentation and line breaks</param>
        /// <returns></returns>
        string AsJson(bool prettyPrinted = false);
        /// <summary>
        /// Attempts to output the query result as a strongly typed object
        /// </summary>
        /// <remarks>This method will using a AutoMapper profile if one has been added to the global Mapper object, otherwise will attempt to dynamically map the query result to the specified type</remarks>
        /// <typeparam name="TEntity">Type into which the query result should be mapped</typeparam>
        /// <returns>List of objects each containing a single query result item</returns>
        IEnumerable<TEntity> ToObject<TEntity>();
    }
}