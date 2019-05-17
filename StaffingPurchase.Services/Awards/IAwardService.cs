using System.Collections.Generic;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.SearchCriteria;

namespace StaffingPurchase.Services.Awards
{
    public interface IAwardService
    {
        IList<Award> GetAll();
        void ImportAwardedUsers(int awardId, IList<string> userList);
        IPagedList<Award> Search(AwardSearchCriteria searchCriteria, PaginationOptions paginationOptions, WorkingUser user);
        void Insert(Award award);
        void Update(Award award);
        void Delete(Award award);
        Award GetById(int id);
    }
}
