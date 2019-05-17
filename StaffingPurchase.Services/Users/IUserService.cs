using System.Collections.Generic;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.SearchCriteria;

namespace StaffingPurchase.Services.Users
{
    public partial interface IUserService
    {
        /// <summary>
        ///     Delete user
        /// </summary>
        /// <param name="entity">User instance with User's Id need to be deleted</param>
        void DeleteUser(User entity);

        /// <summary>
        ///     Delete user
        /// </summary>
        /// <param name="id">User Id</param>
        void DeleteUser(int id);

        /// <summary>
        ///     Gets all roles in system.
        /// </summary>
        /// <returns></returns>
        IDictionary<short, string> GetAllRoles();

        /// <summary>
        ///     Gets all users.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IPagedList<User> GetAllUsers(int pageIndex, int pageSize);

        IList<User> GetAllUsers(int? locationId = null, int? departmentId = null);

        /// <summary>
        ///     Gets roles by username.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        UserRole[] GetRolesByUserName(string username);

        /// <summary>
        ///     Gets user by id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        User GetUserById(int userId);

        /// <summary>
        ///     Gets user by userName.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        User GetUserByUsername(string userName);

        /// <summary>
        ///     Get User by Warehouse ID and User ID
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="locationId"></param>
        /// <param name="userId"></param>
        /// <returns>List of user</returns>
        IPagedList<User> GetUsers(int pageIndex, int pageSize, int? locationId, int userId);

        /// <summary>
        ///     Inserts a user.
        /// </summary>
        /// <param name="user"></param>
        void InsertUser(User user);

        /// <summary>
        ///     Updates a user.
        /// </summary>
        /// <param name="user"></param>
        void UpdateUser(User user);

        /// <summary>
        ///     Reset user's password.
        ///     Default setting is only use username as password.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="password">New password</param>
        /// <param name="useUsername">Specify whether to use username as password or not</param>
        void ReserUserPassword(int userId, string password = null, bool useUsername = true);

        /// <summary>
        ///     Get all users with paging collection
        /// </summary>
        /// <param name="pagingOptions">Define paging options for returned collection</param>
        /// <param name="filter">Filter specific users</param>
        /// <param name="workingUser">The working user</param>
        /// <param name="includeReferences">Indicates if should include referenced tables</param>
        /// <returns></returns>
        IPagedList<User> GetAllUsers(PaginationOptions pagingOptions, UserSearchCriteria filter, WorkingUser workingUser, bool includeReferences = false);
    }
}
