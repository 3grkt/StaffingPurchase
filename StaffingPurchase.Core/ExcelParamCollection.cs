using System.Collections.Generic;

namespace StaffingPurchase.Core
{
    /// <summary>
    /// Contains parameter used in an excel template file.
    /// </summary>
    public class ExcelParamCollection : Dictionary<string, string>
    {
        public string TemplateName { get; set; }

        public ExcelParamCollection()
            : base()
        {
        }
    }
}