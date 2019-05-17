using System.Collections.Generic;
using System.Data;
using System.IO;
using StaffingPurchase.Core.DTOs.Report;

namespace StaffingPurchase.Services.ImportExport
{
    public interface IExportManager
    {
        byte[] ExportToExcelFromDataTable(DataTable tbl, string sheetName = "Data", int[] columnWidths = null);

        byte[] ExportExcelFromMultiDataTable(List<DataTable> tblLst, List<string> workSheetNames,
            List<IDictionary<string, string>> additionalCellsList, Stream templateStream, string startingCell,
            int workSheetIndex = 1);

        byte[] ExportToExcelFromDataTable(DataTable tbl, IDictionary<string, string> additionalCells,
            Stream templateStream, string startingCell, int workSheetIndex = 1);

        byte[] ExportToExcelFromDataTable(DataTable tbl, IDictionary<string, string> additionalCells,
            IList<string> summaryColumns, string cellColor, Stream templateStream, string startingCell,
            bool isProtectedWithPassword);

        byte[] ExportToExcel(List<ExcelWorkSheetDataSource> dataSources, Stream templateStream);
    }
}