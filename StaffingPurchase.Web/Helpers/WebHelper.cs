using System;
using System.IO;
using System.Web;
using StaffingPurchase.Core;
using System.Web.Hosting;

namespace StaffingPurchase.Web.Helpers
{
    public class WebHelper : IWebHelper
    {
        /// <summary>
        /// Format of excel file name.
        /// </summary>
        /// <remarks>
        /// {0} : Type of data (e.g. Asset, Inventory).
        /// {1} : Export date-time.
        /// {2} : Extensions
        /// </remarks>
        public string ExcelFileNameFormat
        {
            get { return "{0}_{1:yyyyMMddHHmmss}{2}"; }
        }

        /// <summary>
        /// Date format.
        /// </summary>
        public string DateFormat
        {
            get { return "dd/MM/yyyy"; }
        }

        /// <summary>
        /// Date time format.
        /// </summary>
        public string DateTimeFormat
        {
            get { return "dd/MM/yyyy HH:mm:ss"; }
        }

        public string ExcelExportTimeFormat
        {
            get { return "HH:mm:ss dd/MM/yyyy"; }
        }

        public string NumberFormat
        {
            get { return "N0"; }
        }

        public string CurrencyFormat
        {
            get { return "N0"; }
        }

        /// <summary>
        /// Maximum of rows contained in a sheet.
        /// </summary>
        public int ExcelSheetMaxRows
        {
            get { return 60000; }
        }

        public string ExcelTemplateFolder
        {
            get { return HostingEnvironment.MapPath("~/App_Data/ExcelTemplates"); }
        }

        //public string InventoryCloseExcelTemplate { get { return Path.Combine(ExcelTemplateFolder, "InventoryClose.xlsx"); } }

        public string DistributorUpdateExcelTemplate
        {
            get { return Path.Combine(ExcelTemplateFolder, "NotScannedDistributorUpdates.xlsx"); }
        }

        public string ProfileBoxDetailExcelTemplate
        {
            get { return Path.Combine(ExcelTemplateFolder, "ProfileBoxDetail.xlsx"); }
        }

        public string ExcelParamConfigFile
        {
            get { return HostingEnvironment.MapPath("~/App_Data/ExcelParams.xml"); }
        }

        #region Methods

        public string GetDateString(DateTime dateTime)
        {
            return dateTime.ToString(this.DateFormat);
        }

        public string GetDateString(DateTime? dateTime)
        {
            if (dateTime == null)
                return string.Empty;

            return GetDateString(dateTime.Value);
        }

        public string GetDateTimeString(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return string.Empty;

            return dateTime.Value.ToString(this.DateTimeFormat);
        }

        #endregion
    }
}
