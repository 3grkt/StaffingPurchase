using System;
using System.Web.Http.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;

namespace StaffingPurchase.Web.Framework.Filters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class JsonCamelCaseConfigAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            //remove the existing Json formatter as this is the global formatter and changing any setting on it
            //would effect other controllers too.
            controllerSettings.Formatters.Remove(controllerSettings.Formatters.JsonFormatter);

            JsonMediaTypeFormatter jsonMediaTypeFormatter = new JsonMediaTypeFormatter();
            jsonMediaTypeFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonMediaTypeFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            controllerSettings.Formatters.Insert(0, jsonMediaTypeFormatter);
        }
    }
}
