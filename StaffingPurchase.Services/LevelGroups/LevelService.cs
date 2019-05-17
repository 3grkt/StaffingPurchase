using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using System.Collections.Generic;
using System.Linq;
using StaffingPurchase.Data;

namespace StaffingPurchase.Services.LevelGroups
{
    public class LevelService : ILevelService
    {
        private readonly IRepository<Level> _levelRepository;
        private readonly IDbContext _dbContext;

        public LevelService(IRepository<Level> levelRepository,IDbContext dbContext)
        {
            _levelRepository = levelRepository;
            _dbContext = dbContext;
        }

        public IEnumerable<Level> GetAllLevels()
        {
            return _levelRepository.TableNoTracking.AsEnumerable();
        }

        public Level GetByLevelCode(string levelCode)
        {
            var query = _levelRepository.TableNoTracking;
            return query.FirstOrDefault(x => x.Name == levelCode); // TODO: consider adding LevelCode to database
        }

        public Level GetLevelById(int id)
        {
            return _levelRepository.GetById(id);
        }

        public IEnumerable<Level> GetLevelsByGroupId(int groupId)
        {
            return _levelRepository.TableNoTracking.Where(x => x.GroupId == groupId).AsEnumerable();
        }

        public void UpdateLevel(Level level)
        {
            if (level != null)
            {
                _levelRepository.Update(level);
            }
        }

        public void UpdateMultiLevels(IEnumerable<Level> levels)
        {
            using (var transaction = _dbContext.BeginDbTransaction())
            {
                try
                {
                    var oldLevels = GetAllLevels().Select(x => new { x.Id, x.GroupId });
                    var results = from t in levels
                                  join l in oldLevels
                                      on t.Id equals l.Id
                                  where t.GroupId != l.GroupId
                                  select t;

                    foreach (var result in results)
                    {
                        var level = GetLevelById(result.Id);
                        level.GroupId = result.GroupId;
                        UpdateLevel(level);
                    }

                    transaction.Commit();
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
               
        }
    }
}