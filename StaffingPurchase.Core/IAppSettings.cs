using System;
using System.Collections.Generic;

namespace StaffingPurchase.Core
{
    /// <summary>
    /// Contains settings stored in App.Config/Web.Config file.
    /// </summary>
    public interface IAppSettings
    {
        /// <summary>
        /// Gets the replacement of test user.
        /// </summary>
        string TestUserReplacement { get; }

        /// <summary>
        /// Gets list of test users.
        /// </summary>
        IList<string> TestUsers { get; }

        /// <summary>
        /// Gets default cache duration in minutes.
        /// </summary>
        int DefaultCacheDuration { get; }

        /// <summary>
        /// Gets number of months order session lasts.
        /// </summary>
        int OrderSessionDurationInMoth { get; }

        /// <summary>
        /// Gets flag indicating whether EmployeeInfoSyncWorker is enabled.
        /// </summary>
        bool EmployeeInfoSyncWorkerEnabled { get; }

        /// <summary>
        /// Gets flag indicating whethere DataUpdateWorker is enabled.
        /// </summary>
        bool DataUpdateWorkerEnabled { get; }

        /// <summary>
        /// Gets name of assembly containing batch jobs.
        /// </summary>
        string BatchJobAssemblyName { get; }

        /// <summary>
        /// Gets name of user who can run batch job api.
        /// </summary>
        string BatchJobRunUser { get; }

        /// <summary>
        /// Gets secret string representing the key which is required to run batch job
        /// </summary>
        string BatchJobRunKey { get; }

        /// <summary>
        /// Gets date format.
        /// </summary>
        string DateFormat { get; }

        /// <summary>
        /// Gets date time format.
        /// </summary>
        string DateTimeFormat { get; }

        /// <summary>
        /// Gets the algorithm to hash password.
        /// </summary>
        string PasswordHashAlgorithm { get; }

        /// <summary>
        /// Gets relative path of SQL file used to query Cadena database.
        /// </summary>
        string CadenaQueryPath { get; }

        /// <summary>
        /// Gets the Excel template for warehouse order reports
        /// </summary>        
        string WarehouseOrderSummaryTemplate { get; }

        /// <summary>
        /// Gets the Excel template for Administrator-view packaged order report
        /// </summary>
        string OrderPackagedAdminReportTemplate { get; }

        /// <summary>
        /// Gets the Excel template for Warehouse-view packaged order report
        /// </summary>
        string OrderPackagedWarehouseReportTemplate { get; }

        /// <summary>
        /// Gets the Excel template for Administrator-view order detail report
        /// </summary>
        string OrderDetailReportTemplate { get; }

        string InternalRequisitionFormTemplate { get; }

        /// <summary>
        /// Gets the sample template for Award Upload feature.
        /// </summary>
        string AwardUploadSampleTemplate { get; }

        /// <summary>
        /// Gets the sample template for Product Upload feature.
        /// </summary>
        string ProductUploadSampleTemplate { get; }
        string SummaryDiscountProductTemplate { get; }
        string OrderByIndividualPVTemplate { get; }
        string OrderByIndividualDiscountTemplate { get; }
        string WarehousePackageOrderTemplate { get; }
        string WarehousePackageDiscountOrderTemplate { get; }
        string WarehousePackagePVOrderTemplate { get; }

        /// <summary>
        /// Gets the path of folder containting policy documents.
        /// </summary>
        string PolicyDocumentFolder { get; }

        /// <summary>
        /// Gets the password to unlock excel editability.
        /// </summary>
        string ExcelProtectionPassword { get; }

        /// <summary>
        /// Get Host Info for Smtp Client
        /// </summary>
        string SmtpClientHost { get; }
        /// <summary>
        /// Get Port Number for Smtp Client
        /// </summary>
        int SmtpClientPort { get; }
        /// <summary>
        /// Get User Credential for Smtp Client
        /// </summary>
        string SmtpClientUser { get; }
        /// <summary>
        /// Get Password Credential for Smtp Client
        /// </summary>
        string SmtpClientPassword { get; }

        string SmtpClientDomain { get; }
        string SmtpClientEmailFrom { get; }

        string PackageAlertEmailTemplate { get; }
		/// <summary>
        /// Gets default culture applied to current application.
        /// </summary>
		string DefaultAppCulture { get; }

        bool SmtpClientEnableSsl { get; }
        int SmtpClientTimeout { get; }
        bool TurnOffCertificateValidation { get; }
    }
}
