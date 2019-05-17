using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using StaffingPurchase.Core;
using StaffingPurchase.Data;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;

namespace StaffingPurchase.Services.LevelGroups
{
    public class LevelGroupService : ILevelGroupService
    {
        private readonly IRepository<LevelGroup> _levelGroupRepository;
        private readonly ILevelService _levelService;
        private readonly IDbContext _dbContext;
        private readonly IResourceManager _resourceManager;
        private readonly ILogger _logger;

        public LevelGroupService(IRepository<LevelGroup> levelGroupRepository, ILevelService levelService, IDbContext dbContext, IResourceManager resourceManager, ILogger logger)
        {
            _levelGroupRepository = levelGroupRepository;
            _levelService = levelService;
            _dbContext = dbContext;
            _resourceManager = resourceManager;
            _logger = logger;
        }

        public void DeleteLevelGroup(int groupId)
        {
            if (!_levelService.GetLevelsByGroupId(groupId).Any())
            {
                var levelGroup = _levelGroupRepository.GetById(groupId);
                _levelGroupRepository.Delete(levelGroup);
            }
            else
            {
                throw new StaffingPurchaseException(_resourceManager.GetString("LevelGroup.DeleteExisted"));
            }
        }

        public IEnumerable<LevelGroup> GetAllLevelGroups()
        {
            return _levelGroupRepository.TableNoTracking.AsEnumerable();
        }

        public LevelGroup GetLevelGroupById(int groupId)
        {
            return _levelGroupRepository.GetById(groupId);
        }

        public void InsertLevelGroup(LevelGroup group)
        {
            _levelGroupRepository.Insert(@group);
        }

        public void UpdateLevelGroup(LevelGroup group)
        {
            _levelGroupRepository.Update(group);
        }

        public void UpdateMultiLevelGroups(IEnumerable<LevelGroup> levelGroups)
        {
            using (var transaction = _dbContext.BeginDbTransaction())
            {
                try
                {
                    var oldLevelGroups = GetAllLevelGroups().Select(x => new { x.Id, x.PV });

                    var results = from t in levelGroups
                                  join l in oldLevelGroups
                                      on t.Id equals l.Id
                                  where Math.Abs(t.PV - l.PV) > float.Epsilon
                                  select t;

                    foreach (var result in results)
                    {
                        var efLevelGroup = GetLevelGroupById(result.Id);
                        efLevelGroup.PV = result.PV;
                        UpdateLevelGroup(efLevelGroup);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
               
        }
    }
}