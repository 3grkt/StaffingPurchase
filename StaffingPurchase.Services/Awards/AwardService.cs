using System;
using System.Collections.Generic;
using System.Linq;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.SearchCriteria;
using StaffingPurchase.Services.Caching;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.PV;

namespace StaffingPurchase.Services.Awards
{
    public class AwardService : ServiceBase, IAwardService
    {
        #region Fields
        private readonly IRepository<Award> _awardRepository;
        private readonly IRepository<User> _userRepository;
        private readonly ICacheService _cacheService;
        private readonly IPvLogService _pvLogService;
        private readonly IResourceManager _resourceManager;

        #endregion

        #region Ctors
        public AwardService(
            IRepository<Award> awardRepository,
             IRepository<User> userRepository,
            ICacheService cacheService,
            IPvLogService pvLogService,
            IResourceManager resourceManager)
        {
            _awardRepository = awardRepository;
            _userRepository = userRepository;
            _cacheService = cacheService;
            _pvLogService = pvLogService;
            _resourceManager = resourceManager;
        }

        #endregion

        #region Services
        public IList<Award> GetAll()
        {
            var allAwards = _cacheService.Get<IList<Award>>(CacheNames.Awards);
            if (allAwards == null)
            {
                allAwards = _awardRepository.TableNoTracking.ToList();
                _cacheService.Set(CacheNames.Awards, allAwards);
            }
            return allAwards;
        }

        public void ImportAwardedUsers(int awardId, IList<string> userList)
        {
            var award = _awardRepository.GetById(awardId);
            if (award == null)
            {
                throw new StaffingPurchaseException(_resourceManager.GetString("Award.NotFound"));
            }

            var updatedUsers = _userRepository.Table.Where(x => userList.Contains(x.UserName)).ToList();
            foreach (var user in updatedUsers)
            {
                user.CurrentPV += award.PV;
                _pvLogService.Log(
                    user.Id,
                    user.UserName,
                    award.PV,
                    string.Format(_resourceManager.GetString("PVLog.Description.UserAwarded"), award.Name),
                    DateTime.Now,
                    PvLogType.Award,
                    user.CurrentPV,
                    true);
            }

            _userRepository.SaveChanges();
        }

        public IPagedList<Award> Search(AwardSearchCriteria searchCriteria, PaginationOptions paginationOptions, WorkingUser user)
        {
            var query = _awardRepository.TableNoTracking;

            if (!string.IsNullOrEmpty(searchCriteria.Name))
            {
                query = query.Where(x => x.Name.Contains(searchCriteria.Name));
            }

            if (searchCriteria.Pv > 0)
            {
                query = query.Where(x => x.PV == searchCriteria.Pv);
            }

            if (string.IsNullOrEmpty(paginationOptions.Sort))
            {
                query = query.OrderBy(x => x.Name);
            }
            else
            {
                query = query.SortBy(paginationOptions.SortExpression);
            }

            return new PagedList<Award>(query, paginationOptions.PageIndex, paginationOptions.PageSize);
        }

        public void Insert(Award award)
        {
            _awardRepository.Insert(award);
            _cacheService.Remove(CacheNames.Awards);
        }

        public void Update(Award award)
        {
            _awardRepository.Update(award);
            _cacheService.Remove(CacheNames.Awards);
        }

        public void Delete(Award award)
        {
            _awardRepository.Delete(award);
            _cacheService.Remove(CacheNames.Awards);
        }

        public Award GetById(int id)
        {
            return _awardRepository.Table.FirstOrDefault(x => x.Id == id);
        }
        #endregion
    }
}
