using System;

namespace StaffingPurchase.Core
{
    public interface IWebHelper
    {
        #region Properties

        /// <summary>
        /// Format of excel file name.
        /// </summary>
        /// <remarks>
        /// {0} : Type of data (e.g. Asset, Inventory).
        /// {1} : Export date-time.
        /// {2} : Extensions
        /// </remarks>
        string ExcelFileNameFormat { get; }

        /// <summary>
        /// Date format.
        /// </summary>
        string DateFormat { get; }

        /// <summary>
        /// Date time format.
        /// </summary>
        string DateTimeFormat { get; }

        /// <summary>
        /// Number format.
        /// </summary>
        string NumberFormat { get; }

        /// <summary>
        /// Currency format.
        /// </summary>
        string CurrencyFormat { get; }

        //string ExcelExportTimeFormat { get; }

        /// <summary>
        /// Maximum of rows contained in a sheet.
        /// </summary>
        int ExcelSheetMaxRows { get; }

        /// <summary>
        /// Folder contains excel template files.
        /// </summary>
        string ExcelTemplateFolder { get; }

        ///// <summary>
        ///// Template file of InventoryClose report.
        ///// </summary>
        //string InventoryCloseExcelTemplate { get; }

        /// <summary>
        /// Template file of DistributorUpdate export.
        /// </summary>
        string DistributorUpdateExcelTemplate { get; }

        /// <summary>
        /// Configuration file of excel parameters.
        /// </summary>
        string ExcelParamConfigFile { get; }

        string ProfileBoxDetailExcelTemplate { get; }

        #endregion

        #region Methods

        string GetDateString(DateTime dateTime);

        string GetDateString(DateTime? dateTime);

        string GetDateTimeString(DateTime? datetime);

        #endregion
    }
}
