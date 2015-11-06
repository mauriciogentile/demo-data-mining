using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data;
using System.Data.OleDb;

namespace Demo.Data
{
    public static class PropertyPredictor
    {
        private static readonly string ConnectionString;
        static readonly ConcurrentDictionary<string, int> _cache = new ConcurrentDictionary<string, int>();

        static PropertyPredictor()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["DataMining"].ConnectionString;
        }

        public static int PredictPrice(this Property property)
        {
            var query = string.Format(Queries.PricePrediction, property.ToSingletonQuery());
            return _cache.GetOrAdd(query, key => Convert.ToInt32(ExecuteScalar(query)));
        }

        public static int PredictSize(this Property property)
        {
            var query = string.Format(Queries.SizePrediction, property.ToSingletonQuery());
            return _cache.GetOrAdd(query, key => Convert.ToInt32(ExecuteScalar(query)));
        }

        static double ExecuteScalar(string query)
        {
            using (var connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = query;
                var result = Convert.ToDouble(command.ExecuteScalar() ?? 0);
                return result < 0 ? 0 : result;
            }
        }
    }
}
