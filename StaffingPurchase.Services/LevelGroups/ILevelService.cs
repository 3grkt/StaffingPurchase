using StaffingPurchase.Core.Domain;
using System.Collections.Generic;

namespace StaffingPurchase.Services.LevelGroups
{
    public interface ILevelService
    {
        IEnumerable<Level> GetAllLevels();

        Level GetLevelById(int id);

        IEnumerable<Level> GetLevelsByGroupId(int groupId);

        Level GetByLevelCode(string levelCode);

        void UpdateLevel(Level level);

        void UpdateMultiLevels(IEnumerable<Level> levels);
    }
}