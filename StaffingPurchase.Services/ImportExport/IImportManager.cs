using StaffingPurchase.Core.DTOs;
using System.Collections.Generic;
using System.IO;

namespace StaffingPurchase.Services.ImportExport
{
    public interface IImportManager
    {
        /// <summary>
        /// Imports product list.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        ProductImportData ImportProductList(Stream stream, string sheetName);

        /// <summary>
        /// Imports award list; this returns a list of employee id.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        IList<string> ImportAwardList(Stream stream, string sheetName);
    }
}