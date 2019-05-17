using StaffingPurchase.Core;

namespace StaffingPurchase.Services.ImportExport
{
    public interface IExcelParamManager
    {
        ExcelParamCollection GetExcelParams(string templateName);
    }
}