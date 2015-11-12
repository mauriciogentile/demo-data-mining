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
        [HttpGet]
        [Route("api/property/size/predict")]
        public Prediction ValidateSize([FromUri]Property property)
        {
            return property.PredictSize();
        }

        [HttpGet]
        [Route("api/property/price/predict")]
        public Prediction PredictPrice([FromUri]Property property)
        {
            return property.PredictPrice();
        }

        [HttpGet]
        [Route("api/property/validate/{propertyTye}")]
        public HttpResponseMessage ValidateBedrooms(PropertyType propertyTye, short bathrooms)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
