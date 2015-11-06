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
        public HttpResponseMessage ValidateSize([FromUri]Property property)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(property.PredictSize().ToString("D0"))
            };
        }

        [HttpGet]
        [Route("api/property/price/predict")]
        public HttpResponseMessage PredictPrice([FromUri]Property property)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(property.PredictPrice().ToString("D0"))
            };
        }

        [HttpGet]
        [Route("api/property/validate/{propertyTye}")]
        public HttpResponseMessage ValidateBedrooms(PropertyType propertyTye, short bathrooms)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
