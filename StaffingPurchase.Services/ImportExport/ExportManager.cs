using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using StaffingPurchase.Core;
using StaffingPurchase.Core.DTOs.Report;

namespace StaffingPurchase.Services.ImportExport
{
    public class ExportManager : IExportManager
    {
        private readonly IAppSettings _appSettings;

        public ExportManager(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        #region IExportManager Members

        public byte[] ExportToExcelFromDataTable(DataTable tbl, string sheetName = "Data", int[] columnWidths = null)
        {
            byte[] excelData = {};

            // Generate excel
            using (var pck = new ExcelPackage())
            {
                //Create the worksheet
                var ws = pck.Workbook.Worksheets.Add(sheetName);

                //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                ws.Cells["A1"].LoadFromDataTable(tbl, true);

                // Set column width
                if (columnWidths != null)
                {
                    for (var i = 0; i < columnWidths.Length; i++)
                    {
                        ws.Column(i + 1).Width = columnWidths[i];
                    }
                }

                excelData = pck.GetAsByteArray();
            }
            return excelData;
        }

        public byte[] ExportToExcelFromDataTable(DataTable tbl, IDictionary<string, string> additionalCells,
            Stream templateStream, string startingCell, int workSheetIndex = 1)
        {
            byte[] excelData = {};
            using (var pck = new ExcelPackage(templateStream))
            {
                var ws = pck.Workbook.Worksheets[workSheetIndex];
                ws.Cells[startingCell].LoadFromDataTable(tbl, false);
                foreach (var cell in additionalCells)
                {
                    ws.Cells[cell.Key].Value = cell.Value;
                }

                excelData = pck.GetAsByteArray();
            }

            return excelData;
        }

        public byte[] ExportToExcelFromDataTable(DataTable tbl, IDictionary<string, string> additionalCells,
            IList<string> summaryColumns, string cellColor, Stream templateStream, string startingCell,
            bool isProtectedWithPassword = false)
        {
            byte[] excelData;
            if (tbl.Rows.Count == 0)
            {
                throw new StaffingPurchaseException("Report.Exception.NoRecord");
            }

            using (var pck = new ExcelPackage(templateStream))
            {
                var ws = pck.Workbook.Worksheets[1];
                ws.Cells[startingCell].LoadFromDataTable(tbl, false);
                var summaryRowIndex =
                    ws.Cells[ws.Cells[startingCell].Start.Row, ws.Cells[startingCell].Columns].Start.Row +
                    tbl.Rows.Count;
                var summaryColIndex =
                    ws.Cells[ws.Cells[startingCell].Start.Row, ws.Cells[startingCell].Columns].Start.Column;
                if (tbl.Rows.Count > 0)
                {
                    foreach (DataColumn column in tbl.Columns)
                    {
                        if (summaryColumns.Contains(column.ColumnName))
                        {
                            var value = tbl.Compute("Sum(" + column.ColumnName + ")", "");
                            ws.Cells[summaryRowIndex, summaryColIndex + column.Ordinal].Value = value;
                        }
                    }
                }

                if (isProtectedWithPassword)
                {
                    ws.Protection.IsProtected = true;
                    ws.Protection.AllowSelectLockedCells = false;
                    ws.Protection.AllowSelectUnlockedCells = false;
                    ws.Protection.SetPassword(_appSettings.ExcelProtectionPassword);
                }


                // Set Background Color
                var color = ColorTranslator.FromHtml(cellColor);
                var summaryCells =
                    ws.Cells[summaryRowIndex, summaryColIndex, summaryRowIndex, summaryColIndex + tbl.Columns.Count];
                summaryCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                summaryCells.Style.Font.Bold = true;
                summaryCells.Style.Fill.BackgroundColor.SetColor(color);

                // Fill with borders
                var dataRange = ws.Cells[
                    ws.Cells[startingCell].Start.Row,
                    ws.Cells[startingCell].Start.Column,
                    summaryRowIndex,
                    summaryColIndex + tbl.Columns.Count];
                dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                foreach (var cell in additionalCells)
                {
                    ws.Cells[cell.Key].Value = cell.Value;
                }

                excelData = pck.GetAsByteArray();
            }

            return excelData;
        }

        public byte[] ExportToExcel(List<ExcelWorkSheetDataSource> dataSources, Stream templateStream)
        {
            byte[] bin;
            using (var pck = new ExcelPackage(templateStream))
            {
                foreach (var wsSource in dataSources)
                {
                    var ws = pck.Workbook.Worksheets[wsSource.WorkSheetName];
                    var totalColumns = wsSource.Table.Columns.Count;
                    ws.Cells[wsSource.StartingCell].LoadFromDataTable(wsSource.Table, false);
                    var summaryRowIndex =
                        ws.Cells[ws.Cells[wsSource.StartingCell].Start.Row, ws.Cells[wsSource.StartingCell].Columns]
                            .Start.Row +
                        wsSource.Table.Rows.Count;
                    var summaryColIndex =
                        ws.Cells[ws.Cells[wsSource.StartingCell].Start.Row, ws.Cells[wsSource.StartingCell].Columns]
                            .Start.Column;
                    if (wsSource.Table.Rows.Count > 0)
                    {
                        foreach (DataColumn column in wsSource.Table.Columns)
                        {
                            if (wsSource.SummaryColumnHeaders.Contains(column.ColumnName))
                            {
                                var value = wsSource.Table.Compute("Sum(" + column.ColumnName + ")", "");
                                ws.Cells[summaryRowIndex, summaryColIndex + column.Ordinal].Value = value;
                            }
                        }
                    }

                    if (wsSource.IsProtectedWithPassword)
                    {
                        ws.Protection.IsProtected = true;
                        ws.Protection.AllowSelectLockedCells = false;
                        ws.Protection.AllowSelectUnlockedCells = false;
                        ws.Protection.SetPassword(_appSettings.ExcelProtectionPassword);
                    }

                    // Set Background Color
                    var color = ColorTranslator.FromHtml(wsSource.SummaryCellColor);
                    var summaryCells =
                        ws.Cells[summaryRowIndex, summaryColIndex, summaryRowIndex, summaryColIndex + totalColumns];
                    summaryCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    summaryCells.Style.Font.Bold = true;
                    summaryCells.Style.Fill.BackgroundColor.SetColor(color);

                    // Fill with borders
                    var dataRange = ws.Cells[
                        ws.Cells[wsSource.StartingCell].Start.Row,
                        ws.Cells[wsSource.StartingCell].Start.Column,
                        summaryRowIndex,
                        summaryColIndex + totalColumns];
                    dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    foreach (var cell in wsSource.AdditionalCells)
                    {
                        ws.Cells[cell.Key].Value = cell.Value;
                    }
                }

                bin = pck.GetAsByteArray();
            }

            return bin;
        }

        public byte[] ExportExcelFromMultiDataTable(List<DataTable> tblLst, List<string> workSheetNames,
            List<IDictionary<string, string>> additionalCellsList, Stream templateStream, string startingCell,
            int workSheetIndex = 1)
        {
            byte[] excelData = {};
            using (var pck = new ExcelPackage(templateStream))
            {
                // Add worksheets with the first worksheet index template
                var originalWorksheet = pck.Workbook.Worksheets[workSheetIndex];
                pck.Workbook.Worksheets[workSheetIndex].Name = workSheetNames[0];
                for (var i = 1; i < tblLst.Count; i++)
                {
                    pck.Workbook.Worksheets.Add(workSheetNames[i], originalWorksheet);
                }

                // Fill data
                for (var i = 0; i < tblLst.Count; i++)
                {
                    var ws = pck.Workbook.Worksheets[i + workSheetIndex];
                    ws.Cells[startingCell].LoadFromDataTable(tblLst[i], false);
                    foreach (var cell in additionalCellsList[i])
                    {
                        ws.Cells[cell.Key].Value = cell.Value;
                    }
                }

                excelData = pck.GetAsByteArray();
            }

            return excelData;
        }

        #endregion IExportManager Members
    }
}
