using System;
using System.Collections.Generic;
using System.Configuration;

namespace StaffingPurchase.Core
{
    public class AppSettings : IAppSettings
    {
        #region IAppSettings Members

        public IList<string> TestUsers
        {
            get
            {
                var users = GetAppSettingString("TestUsers", defaultValue: "TriNguyen");
                return !string.IsNullOrEmpty(users)
                    ? new List<string>(users.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    : new List<string>();
            }
        }

        public string TestUserReplacement
        {
            get { return GetAppSettingString("TestUserReplacement", defaultValue: "VNM00000"); }
        }

        public int DefaultCacheDuration
        {
            get { return 30; }
        }

        public int OrderSessionDurationInMoth
        {
            get { return GetAppSettingInt32("OrderSessionDurationInMoth", 2); }
        }

        public bool EmployeeInfoSyncWorkerEnabled
        {
            get { return GetAppSettingsBoolean("EmployeeInfoSyncWorkerEnabled"); }
        }

        public bool DataUpdateWorkerEnabled
        {
            get { return GetAppSettingsBoolean("DataUpdateWorkerEnabled"); }
        }

        public string BatchJobAssemblyName
        {
            get { return GetAppSettingString("BatchJobAssemblyName", defaultValue: "StaffingPurchase.Jobs"); }
        }

        public string BatchJobRunUser
        {
            get { return GetAppSettingString("BatchJobRunUser", defaultValue: "_power_user"); }
        }

        public string BatchJobRunKey
        {
            get { return GetAppSettingString("BatchJobRunKey", defaultValue: "3A1DA913-429B-4325-93AB-857097C4E542"); }
        }

        public string DateFormat
        {
            get { return GetAppSettingString("DateFormat", defaultValue: "dd/MM/yyyy"); }
        }

        public string DateTimeFormat
        {
            get { return GetAppSettingString("DateTimeFormat", defaultValue: "dd/MM/yyyy HH:mm:ss"); }
        }

        public string PasswordHashAlgorithm
        {
            get { return GetAppSettingString("PasswordHashAlgorithm", defaultValue: "SHA256"); }
        }

        public string CadenaQueryPath
        {
            get { return GetAppSettingString("CadenaQueryPath", defaultValue: "SQL\\CadenaQuery.sql"); }
        }

        public string WarehouseOrderSummaryTemplate
        {
            get
            {
                return GetAppSettingString("WarehouseOrderSummary",
                    defaultValue: "~/App_Data/ExcelTemplates/OrderSummary.xlsx");
            }
        }

        public string OrderPackagedAdminReportTemplate => GetAppSettingString("OrderPackagedAdminReportTemplate",
            defaultValue: "~/App_Data/ExcelTemplates/OrderPackagedAdminReport.xlsx");

        public string OrderPackagedWarehouseReportTemplate => GetAppSettingString("OrderPackagedWarehouseReportTemplate",
            defaultValue: "~/App_Data/ExcelTemplates/OrderPackagedWarehouseReport.xlsx");

        public string OrderDetailReportTemplate => GetAppSettingString("OrderDetailReportTemplate",
            defaultValue: "~/App_Data/ExcelTemplates/OrderDetailReport.xlsx");

        public string InternalRequisitionFormTemplate => GetAppSettingString("InternalRequisitionForm",
            defaultValue: "~/App_Data/ExcelTemplates/InternalRequisitionForm.xlsx");

        public string AwardUploadSampleTemplate => GetAppSettingString("AwardUploadSampleTemplate",
            defaultValue: "~/App_Data/ExcelTemplates/Award-sample.xlsx");

        public string ProductUploadSampleTemplate => GetAppSettingString("ProductUploadSampleTemplate",
            defaultValue: "~/App_Data/ExcelTemplates/ProductList-sample.xlsx");

        public string SummaryDiscountProductTemplate => GetAppSettingString("SummaryDiscountProductTemplate",
            defaultValue: "~/App_Data/ExcelTemplates/SummaryDiscountProduct.xlsx");

        public string OrderByIndividualPVTemplate => GetAppSettingString("OrderByIndividualPVTemplate",
            defaultValue: "~/App_Data/ExcelTemplates/OrderByIndividualPV.xlsx");

        public string OrderByIndividualDiscountTemplate => GetAppSettingString("OrderByIndividualDiscountTemplate",
            defaultValue: "~/App_Data/ExcelTemplates/OrderByIndividualDiscount.xlsx");

        public string WarehousePackageOrderTemplate
            =>
                GetAppSettingString("WarehousePackageOrderTemplate",
                    defaultValue: "~/App_Data/ExcelTemplates/WarehousePackageOrder.xlsx");

        public string WarehousePackagePVOrderTemplate => GetAppSettingString("WarehousePackageOrderTemplate",
            defaultValue: "~/App_Data/ExcelTemplates/WarehousePackageOrderPV.xlsx");

        public string WarehousePackageDiscountOrderTemplate => GetAppSettingString("WarehousePackageOrderTemplate",
            defaultValue: "~/App_Data/ExcelTemplates/WarehousePackageOrderDiscount.xlsx");

        public string PolicyDocumentFolder
            => GetAppSettingString("PolicyDocumentFolder", defaultValue: "~/App_Data/PolicyDocuments/");

        public string ExcelProtectionPassword
            => GetAppSettingString("ExcelProtectionPassword", defaultValue: "StaffPurchasing@123");

        public string PackageAlertEmailTemplate => GetAppSettingString("PackageAlertEmailTemplate",defaultValue:"~/App_Data/EmailTemplates/PackageEmail.cshtml");

        public string SmtpClientHost => GetAppSettingString("SmtpClientHost");
        public int SmtpClientPort => GetAppSettingInt32("SmtpClientPort", 25);
        public string SmtpClientUser => GetAppSettingString("SmtpClientUser");
        public string SmtpClientPassword => GetAppSettingString("SmtpClientPassword");
        public string SmtpClientDomain => GetAppSettingString("SmtpClientDomain");

        public string SmtpClientEmailFrom
            => GetAppSettingString("SmtpClientEmailFrom");
        public string DefaultAppCulture => GetAppSettingString("DefaultAppCulture", defaultValue: "vi");
        public bool SmtpClientEnableSsl => GetAppSettingsBoolean("SmtpClientEnableSsl");
        public int SmtpClientTimeout => GetAppSettingInt32("SmtpClientTimeout", 10000);
        public bool TurnOffCertificateValidation => GetAppSettingsBoolean("TurnOffCertificateValidation", false);

        #endregion

        #region Utility



        private string GetAppSettingString(string key, bool isRequired = false, string defaultValue = null,
            Func<string, string> settingProcessor = null)
        {
            var setting = ConfigurationManager.AppSettings[key];

            if (!string.IsNullOrEmpty(setting))
                return (settingProcessor == null) ? setting : settingProcessor(setting);

            if (isRequired)
                throw new StaffingPurchaseException(
                    $"The key '{key}' does not exist in configuration file OR the retrieved values is empty");

            if (!string.IsNullOrEmpty(defaultValue))
                return (settingProcessor == null) ? defaultValue : settingProcessor(defaultValue);

            return string.Empty;
        }

        private bool GetAppSettingsBoolean(string key, bool isRequired = false)
        {
            var setting = ConfigurationManager.AppSettings[key];

            if (!string.IsNullOrEmpty(setting))
                return Convert.ToBoolean(setting);

            if (isRequired)
                throw new StaffingPurchaseException(
                    $"The key '{key}' does not exist in configuration file OR the retrieved values is empty");

            return false;
        }

        private int GetAppSettingInt32(string key, int defaultValue)
        {
            int value;
            if (int.TryParse(ConfigurationManager.AppSettings[key], out value))
                return value;
            return defaultValue;
        }

        #endregion
    }
}
