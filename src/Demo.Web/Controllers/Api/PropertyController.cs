using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        [Route("api/property/validate/{propertyTye}")]
        public HttpResponseMessage ValidateBedrooms(PropertyType propertyTye, int bedrooms)
        {
            /*var query = string.Format(Queries.BedroomPrediction, propertyTye);
            var prediction = ExecuteScalar(query);
            var variaton = prediction * AcceptableVariation;
            if (Math.Abs(bedrooms - variaton) > AcceptableVariation)
            {
                return new HttpResponseMessage(HttpStatusCode.NotAcceptable)
                {
                    Content = new StringContent(GetAcceptableRangeString(prediction))
                };
            }*/
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("api/property/validate/price")]
        public HttpResponseMessage ValidateBedrooms([FromUri]Property property)
        {
            var query = string.Format(Queries.PricePrediction, property.ToSingletonQuery());
            var prediction = _cache.GetOrAdd(query, key => Convert.ToInt32(ExecuteScalar(query)));
            var variaton = prediction * AcceptableVariation;
            if (Math.Abs(property.Price - prediction) > variaton)
            {
                return new HttpResponseMessage(HttpStatusCode.NotAcceptable)
                {
                    Content = new StringContent(GetAcceptableRangeString(prediction))
                };
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
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

        static string GetAcceptableRangeString(double value)
        {
            var min = value - (value * AcceptableVariation);
            var max = value + (value * AcceptableVariation);
            return string.Format("Rango aceptable de '{0}' a '{1}'", min, max);
        }
    }
}
