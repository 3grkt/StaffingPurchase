using System;
using System.Collections.Generic;
using System.Xml.Linq;
using StaffingPurchase.Core;
using StaffingPurchase.Services.Caching;

namespace StaffingPurchase.Services.ImportExport
{
    public class ExcelParamManager : IExcelParamManager
    {
        private const string EXCEL_PARAMS_CACHE_NAME = "ExcelParams";
        private readonly IWebHelper _webHelper;
        private readonly ICacheService _cacheService;

        public ExcelParamManager(IWebHelper webHelper, ICacheService cacheService)
        {
            _webHelper = webHelper;
            _cacheService = cacheService;
        }

        #region IExcelParamManager Members

        public ExcelParamCollection GetExcelParams(string templateName)
        {
            ExcelParamCollection collection = null;

            var cachedParams = GetCachedExcelParams();
            cachedParams.TryGetValue(templateName, out collection);

            return collection;
        }

        #endregion

        private IDictionary<string, ExcelParamCollection> GetCachedExcelParams()
        {
            var cachedParams = _cacheService.Get<IDictionary<string, ExcelParamCollection>>(EXCEL_PARAMS_CACHE_NAME);
            if (cachedParams == null)
            {
                cachedParams = new Dictionary<string, ExcelParamCollection>();

                XDocument doc = XDocument.Load(_webHelper.ExcelParamConfigFile);
                var excelParams = doc.Element("ExcelParams");
                if (excelParams != null)
                {
                    foreach (var templateElement in excelParams.Elements("Template"))
                    {
                        var templateName = templateElement.Attribute("name")?.Value;
                        var collection = new ExcelParamCollection() { TemplateName = templateName };
                        foreach (var param in templateElement.Elements("Param"))
                        {
                            var paramName = param.Attribute("name");
                            if (paramName != null)
                            {
                                collection[paramName.Value] = param.Value; // if there are 2 parmas with the same key, last param is used
                            }
                        }

                        cachedParams[templateName] = collection;
                    }
                }

                _cacheService.Set(EXCEL_PARAMS_CACHE_NAME, cachedParams);
            }

            return cachedParams;
        }
    }
}
