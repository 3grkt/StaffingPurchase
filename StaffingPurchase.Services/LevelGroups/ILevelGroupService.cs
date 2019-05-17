using StaffingPurchase.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffingPurchase.Services.LevelGroups
{
    public interface ILevelGroupService
    {
        void DeleteLevelGroup(int groupId);

        IEnumerable<LevelGroup> GetAllLevelGroups();

        LevelGroup GetLevelGroupById(int groupId);

        void InsertLevelGroup(LevelGroup group);

        void UpdateLevelGroup(LevelGroup group);

        void UpdateMultiLevelGroups(IEnumerable<LevelGroup> levelGroups);
    }
}