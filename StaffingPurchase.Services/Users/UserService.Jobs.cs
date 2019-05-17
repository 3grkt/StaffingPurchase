using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.Domain.Cadena;
using StaffingPurchase.Core;
using StaffingPurchase.Data;

namespace StaffingPurchase.Services.Users
{
    public partial class UserService
    {
        #region Methods

        public void SyncUserInfoWithCadena()
        {
            try
            {
                _logger.Debug("Start retrieving Cadena employee info");

                IList<EmployeeInfo> employeeInfoList = _cadenaIntegrationService.GetCadenaEmployeeInfo();

                _logger.Debug("Start syncing locations");
                CheckAndSyncLocations(employeeInfoList);

                _logger.Debug("Start syncing departments");
                CheckAndSyncDepartments(employeeInfoList);

                _logger.Debug("Start syncing employee info");
                IList<User> allUsers = _userRepository.Table.ToList();
                foreach (EmployeeInfo cadenaUser in employeeInfoList)
                {
                    User user = allUsers.FirstOrDefault(x => x.UserName == cadenaUser.EmployeeID);

                    if (user != null)
                    {
                        string syncLog = CheckAndSyncUserInfo(cadenaUser, user);

                        if (!string.IsNullOrEmpty(syncLog))
                        {
                            _logger.Debug($"Synced info for user {user.UserName} - {syncLog}");
                        }
                    }
                    else // insert new user to db
                    {
                        User newUser = SyncNewUser(cadenaUser);
                        _userRepository.Insert(newUser, false);
                        _logger.Debug($"New user {newUser.UserName} was added to database");
                    }
                }

                _logger.Debug("Start committing transaction");
                _userRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to sync user info with Cadena.", ex);
            }
        }

        public void UpdatePvOnBirthday()
        {
            try
            {
                var updatedUsers = _userRepository.Table
                    .Where(x => x.DateOfBirth.Month == DateTime.Now.Month && x.DateOfBirth.Day == DateTime.Now.Day)
                    .ToList();
                _logger.Debug($"Found {updatedUsers.Count} user(s) has birthday today");

                foreach (var user in updatedUsers)
                {
                    user.CurrentPV += _appPolicy.BirthDayAwardedPV;

                    _pvLogService.Log(
                        user.Id,
                        user.UserName,
                        _appPolicy.BirthDayAwardedPV,
                        _resourceManager.GetString("PVLog.Description.AwardedOnBirthday"),
                        DateTime.Now,
                        PvLogType.Birthday,
                        user.CurrentPV,
                        true
                    );
                }

                _logger.Debug("Start committing transaction");
                _userRepository.SaveChanges(); // commit all changes
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to update PV on user's birthday.", ex);
            }
        }

        public void ResetPvOnYearEnds()
        {
            try
            {
                var currentYear = DateTime.Now.Year;
                var lastSessionOfYearEndDate = new DateTime(currentYear, 1, _appPolicy.OrderSessionEndDayOfMonth);
                if (DateTime.Now.Date == lastSessionOfYearEndDate.AddDays(1))
                {
                    _logger.Debug("Start reseting PV on year ends");

                    var allUsers = _userRepository.Table.AsEnumerable();
                    foreach (var user in allUsers)
                    {
                        var currentPv = user.CurrentPV;
                        if (currentPv > 0)
                        {
                            user.CurrentPV = 0;

                            _pvLogService.Log(
                               user.Id,
                               user.UserName,
                               -currentPv,
                               string.Format(_resourceManager.GetString("PVLog.Description.ResetOnYearEnds"), currentYear - 1),
                               DateTime.Now,
                               PvLogType.Reset,
                               0,
                               true
                           );
                        }
                    }

                    _logger.Debug("Start committing transaction");
                    _userRepository.SaveChanges(); // commit all changes
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to reset user's PV on year ends.", ex);
            }
        }

        public void RewardPvMonthly()
        {
            if (DateTime.Now.Day != _appPolicy.OrderSessionStartDayOfMonth) // only process in order session start day
            {
                return;
            }

            try
            {
                _logger.Debug("Start rewarding PV monthly");

                var allUsers = _userRepository.Table
                    .IncludeTable(x => x.Level.LevelGroup)
                    .AsEnumerable();

                foreach (var user in allUsers)
                {
                    var rewardedPv = user.Level?.LevelGroup?.PV ?? 0d;
                    if (rewardedPv <= 0d)
                    {
                        continue;
                    }

                    user.CurrentPV += rewardedPv;

                    _pvLogService.Log(
                       user.Id,
                       user.UserName,
                       rewardedPv,
                       _resourceManager.GetString("PVLog.Description.MonthlyRewarded"),
                       DateTime.Now,
                       PvLogType.MonthlyReward,
                       user.CurrentPV,
                       true
                   );
                }

                _logger.Debug("Start committing transaction");
                _userRepository.SaveChanges(); // commit all changes
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to reset user's PV on year ends.", ex);
            }
        }

        #endregion Methods

        #region Utility
        private string CheckAndSyncUserInfo(EmployeeInfo cadenaUser, User user)
        {
            StringBuilder sb = new StringBuilder();

            if (cadenaUser.FullName != user.FullName)
            {
                user.FullName = cadenaUser.FullName;
                sb.AppendFormat(", FullName: {0}", cadenaUser.FullName);
            }

            if (cadenaUser.DateOfBirth.HasValue && cadenaUser.DateOfBirth != user.DateOfBirth)
            {
                user.DateOfBirth = cadenaUser.DateOfBirth.Value;
                sb.AppendFormat(", DateOfBirth: {0}", cadenaUser.DateOfBirth.Value.ToString(_appSettings.DateFormat));
            }

            if (cadenaUser.ServiceStartDate.HasValue && cadenaUser.ServiceStartDate != user.StartDate)
            {
                user.StartDate = cadenaUser.ServiceStartDate;
                sb.AppendFormat(", ServiceStartDate: {0}", cadenaUser.ServiceStartDate.Value.ToString(_appSettings.DateFormat));
            }

            if (cadenaUser.ResignationDate != user.EndDate)
            {
                user.EndDate = cadenaUser.ResignationDate;
                sb.AppendFormat(", ResignationDate: {0}", cadenaUser.ResignationDate.Value.ToString(_appSettings.DateFormat));
            }

            var locationId = GetLocationIdFromName(cadenaUser.Location);
            if (locationId.HasValue && locationId != user.LocationId)
            {
                user.LocationId = locationId;
                sb.AppendFormat(", Location: {0}", cadenaUser.Location);
            }

            var deparmentId = GetDepartmentIdFromName(cadenaUser.Department);
            if (deparmentId.HasValue && deparmentId != user.DepartmentId)
            {
                user.DepartmentId = deparmentId;
                sb.AppendFormat(", Department: {0}", cadenaUser.Department);
            }

            if (cadenaUser.LevelScale != user.LevelId)
            {
                user.LevelId = cadenaUser.LevelScale;
                sb.AppendFormat(", Level: {0}", cadenaUser.LevelScale);
            }

            if (cadenaUser.CodeCenterCode != user.CostCenter)
            {
                user.CostCenter = cadenaUser.CodeCenterCode;
                sb.AppendFormat(", CostCenter: {0}", cadenaUser.CodeCenterCode);
            }

            if (cadenaUser.EmailAddress != user.EmailAddress)
            {
                user.EmailAddress = cadenaUser.EmailAddress;
                sb.AppendFormat(", EmailAddress: {0}", cadenaUser.EmailAddress);
            }

            if (sb.Length > 0)
            {
                sb.Remove(0, 2);
            }
            return sb.ToString();
        }

        private User SyncNewUser(EmployeeInfo cadenaUser)
        {
            var user = new User()
            {
                UserName = cadenaUser.EmployeeID,
                FullName = cadenaUser.FullName,
                DateOfBirth = cadenaUser.DateOfBirth ?? new DateTime(1753, 1, 1), // if null, use min value of SQLServer
                StartDate = cadenaUser.ServiceStartDate,
                EndDate = cadenaUser.ResignationDate,
                RoleId = (short)UserRole.Employee,
                LevelId = cadenaUser.LevelScale,
                CurrentPV = 0,
                PasswordHash = CommonHelper.HashString(cadenaUser.EmployeeID, _appSettings.PasswordHashAlgorithm),
                CostCenter = cadenaUser.CodeCenterCode,
                EmailAddress = cadenaUser.EmailAddress,
                Language = _appSettings.DefaultAppCulture
            };

            var locationId = GetLocationIdFromName(cadenaUser.Location);
            if (locationId.HasValue)
            {
                user.LocationId = locationId;
            }

            var deparmentId = GetDepartmentIdFromName(cadenaUser.Department);
            if (deparmentId.HasValue)
            {
                user.DepartmentId = deparmentId;
            }

            return user;
        }

        private void CheckAndSyncLocations(IList<EmployeeInfo> employeeInfoList)
        {
            var allLocationNames = employeeInfoList.Select(x => x.Location).Distinct();
            foreach (var locationName in allLocationNames)
            {
                if (_locationService.GetByName(locationName) == null)
                {
                    _locationService.Add(new Location() { Name = locationName });
                    _logger.Debug($"Location '{locationName}' was added to database");
                }
            }
        }

        private void CheckAndSyncDepartments(IList<EmployeeInfo> employeeInfoList)
        {
            var allDepartmentNames = employeeInfoList.Select(x => x.Department).Distinct();
            foreach (var departmentName in allDepartmentNames)
            {
                if (_departmentService.GetByName(departmentName) == null)
                {
                    _departmentService.Add(new Department() { Name = departmentName });
                    _logger.Debug($"Department '{departmentName}' was added to database");
                }
            }
        }

        private int? GetLocationIdFromName(string locationCode)
        {
            var location = _locationService.GetByName(locationCode);
            return location?.Id;
        }

        private int? GetDepartmentIdFromName(string departmentCode)
        {
            var department = _departmentService.GetByName(departmentCode);
            return department?.Id;
        }

        #endregion
    }
}
