using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using StaffingPurchase.Core;
using StaffingPurchase.Services.Email;

namespace StaffingPurchase.Web.Api
{
    [RoutePrefix("api/test/email")]
    public class TestEmailController : ApiControllerBase
    {
        private readonly IEmailDelivery _emailDelivery;
        private readonly IAppSettings _appSettings;

        public TestEmailController(IEmailDelivery emailDelivery, IAppSettings appSettings)
        {
            this._emailDelivery = emailDelivery;
            this._appSettings = appSettings;
        }

        [HttpGet]
        [Route("to/{to}")]
        public HttpResponseMessage TestEmailTo([FromUri] string to)
        {
            _emailDelivery.Send("test", "test", to, _appSettings.SmtpClientEmailFrom);

            return Request.CreateResponse();
        }
    }
}