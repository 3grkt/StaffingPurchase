using StaffingPurchase.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using StaffingPurchase.Services.Logging;

namespace StaffingPurchase.Web.Api
{
    public class TextResourceController : ApiControllerBase
    {
        public TextResourceController(IResourceManager resourceManager, ILogger logger)
            : base(logger, resourceManager)
        {
        }

        [AllowAnonymous]
        public IDictionary<string, string> GetAll()
        {
            return _resourceManager.GetAllTexts(null);
        }
    }
}
