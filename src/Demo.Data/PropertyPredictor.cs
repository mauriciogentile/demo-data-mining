using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data;
using System.Data.OleDb;

namespace Demo.Data
{
    public struct Prediction
    {
        public int Value;
        public int StdDev;
    }

    public static class PropertyPredictor
    {
        private static readonly string ConnectionString;
        static readonly ConcurrentDictionary<string, Prediction> _cache = new ConcurrentDictionary<string, Prediction>();

        static PropertyPredictor()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["DataMining"].ConnectionString;
        }

        public static Prediction PredictPrice(this Property property)
        {
            var query = string.Format(Queries.PricePrediction, property.ToSingletonQuery());
            return _cache.GetOrAdd(query, key => ExecuteScalar(query));
        }

        public static Prediction PredictSize(this Property property)
        {
            var query = string.Format(Queries.SizePrediction, property.ToSingletonQuery());
            return _cache.GetOrAdd(query, key => ExecuteScalar(query));
        }

        static Prediction ExecuteScalar(string query)
        {
            using (var connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = query;
                var reader = command.ExecuteReader();
                if (reader != null && reader.Read())
                {
                    return new Prediction
                    {
                        Value = Convert.ToInt32(reader.GetValue(0)),
                        StdDev = Convert.ToInt32(reader.GetValue(1))
                    };
                }

                return new Prediction();
            }
        }
    }
}
