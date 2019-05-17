using System;
using System.Collections.Generic;
using System.Data;

namespace StaffingPurchase.Core.DTOs.Report
{
    public class ExcelWorkSheetDataSource
    {
        public DataTable Table { get; set; }
        public string WorkSheetName { get; set; }
        public IDictionary<string, string> AdditionalCells { get; set; }
        public IList<string> SummaryColumnHeaders { get; set; }
        public string SummaryCellColor { get; set; }
        public string StartingCell { get; set; }
        public bool IsProtectedWithPassword { get; set; }
    }
}
