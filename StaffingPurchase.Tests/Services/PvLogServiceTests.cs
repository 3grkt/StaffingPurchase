using System;
using NUnit.Framework;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.SearchCriteria;
using StaffingPurchase.Services.Configurations;
using StaffingPurchase.Services.PV;

namespace StaffingPurchase.Tests.Services
{
    public class PvLogServiceTests : TestSuiteBase
    {
        private PvLogService _pvLogService;

        [SetUp]
        public void Setup()
        {
            _pvLogService = new PvLogService(
                ContainerManager.Resolve<IRepository<PVLog>>(DbContextName),
                ContainerManager.Resolve<IDataHelper>(DbContextName),
                ContainerManager.Resolve<IAppSettings>(DbContextName),
                ContainerManager.Resolve<IAppPolicy>(DbContextName));
        }

        [Test, Category(TestCategory.IntegrationTest)]
        [TestCase(466)]
        public void SearchLogSummary_ShouldReturnCorrectData_ForSpecificUser(int userId)
        {
            var criteria = new PvLogSearchCriteria
            {
                StartDate = new DateTime(2017, 1, 1),
                EndDate = new DateTime(2017, 2, 28),
                UserId = userId
            };

            var result = _pvLogService.SearchLogSummary(criteria, new PaginationOptions { PageIndex = 1, PageSize = 10 }, new WorkingUser());

            Assert.Greater(result.TotalCount, 0);

            Console.WriteLine("User \tMonth \tPrevious PV \tMonthly Rewarded \tAwarded \tBirthday \tOrdering \tRemaining");
            foreach (var log in result)
            {
                Console.WriteLine(string.Join(" \t", log.UserName, log.Month, log.PreviousSessionPv, log.MonthlyRewardedPv, log.AwardedPv, log.BirthdayRewardedPv, log.OrderingPv, log.RemainingPv));
            }
        }

        [Test, Category(TestCategory.IntegrationTest)]
        public void SearchLogSummary_ShouldReturnCorrectData_ForAllUser()
        {
            var criteria = new PvLogSearchCriteria
            {
                StartDate = new DateTime(2016, 12, 1),
                EndDate = new DateTime(2017, 1, 31)
            };

            var result = _pvLogService.SearchLogSummary(criteria, new PaginationOptions { PageIndex = 1, PageSize = 10 }, new WorkingUser());

            Assert.Greater(result.TotalCount, 0);

            Console.WriteLine("User \tMonth \tPrevious PV \tMonthly Rewarded \tAwarded \tBirthday \tOrdering \tRemaining");
            foreach (var log in result)
            {
                Console.WriteLine(string.Join(" \t", log.UserName, log.Month, log.PreviousSessionPv, log.MonthlyRewardedPv, log.AwardedPv, log.BirthdayRewardedPv, log.OrderingPv, log.RemainingPv));
            }
        }
    }
}
