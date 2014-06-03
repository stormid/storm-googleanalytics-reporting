using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Storm.GoogleAnalytics.Reporting.Core.Impl
{
    public sealed class GoogleAnalyticsDataResponse : IGoogleAnalyticsDataResponse
    {
        private DataTable _data;
        internal GoogleAnalyticsDataResponse(DataTable data, bool hasSampledData)
        {
            HasSampledData = hasSampledData;
            _data = data;
        }

        public bool HasSampledData { get; private set; }

        public DataTable AsDataTable()
        {
            return _data;
        }

        public string AsJson(bool prettyPrinted = false)
        {
            try
            {
                return AsDataTable() != null
                    ? JsonConvert.SerializeObject(AsDataTable(), prettyPrinted ? Formatting.Indented : Formatting.None,
                        new DataTableConverter())
                    : null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to map query response to JSON", ex);
            }

        }

        public IEnumerable<TEntity> ToObject<TEntity>()
        {
            try
            {
                return AsDataTable() != null
                    ? Mapper.DynamicMap<IDataReader, IEnumerable<TEntity>>(AsDataTable().CreateDataReader())
                    : Enumerable.Empty<TEntity>();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to map query response to specified type", ex);
            }
        }
    }
}