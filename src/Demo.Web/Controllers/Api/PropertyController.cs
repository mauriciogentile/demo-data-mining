using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Demo.Data;

namespace Demo.Web.Controllers.Api
{
    public class PropertyController : ApiController
    {
        private static readonly string ConnectionString;
        private const double AcceptableVariation = 0.1;
        static readonly ConcurrentDictionary<string, int> _cache = new ConcurrentDictionary<string, int>();

        static PropertyController()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["DataMining"].ConnectionString;
        }

        [HttpGet]
        [Route("api/property/size/predict")]
        public HttpResponseMessage ValidateSize([FromUri]Property property)
        {
            var query = string.Format(Queries.SizePrediction, property.ToSingletonQuery());
            var prediction = _cache.GetOrAdd(query, key => Convert.ToInt32(ExecuteScalar(query)));
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(prediction.ToString("D0"))
            };
        }

        [HttpGet]
        [Route("api/property/price/predict")]
        public HttpResponseMessage PredictPrice([FromUri]Property property)
        {
            var query = string.Format(Queries.PricePrediction, property.ToSingletonQuery());
            var prediction = _cache.GetOrAdd(query, key => Convert.ToInt32(ExecuteScalar(query)));
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(prediction.ToString("D0"))
            };
        }

        [HttpGet]
        [Route("api/property/validate/{propertyTye}")]
        public HttpResponseMessage ValidateBedrooms(PropertyType propertyTye, short bathrooms)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
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
