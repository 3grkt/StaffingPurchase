using System;
using System.Linq;
using System.Linq.Expressions;

namespace StaffingPurchase.Core.Data
{
    /// <summary>
    /// Repository
    /// </summary>
    public partial interface IRepository<T> where T : EntityBase
    {
        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        T GetById(object id);

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="commitChanges">true: save change to db immediately; false: not save yet</param>
        void Insert(T entity, bool commitChanges = true);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="commitChanges">true: save change to db immediately; false: not save yet</param>
        void Update(T entity, bool commitChanges = true);

        /// <summary>
        /// Bulk update entities
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <param name="updateExpression"></param>
        /// <returns>Number of records updated</returns>
        int BulkUpdate(Expression<Func<T, bool>> filterExpression, Expression<Func<T, T>> updateExpression);

        /// <summary>
        /// Bulk delete entities
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        int BulkDelete(Expression<Func<T, bool>> filterExpression);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="commitChanges">true: save change to db immediately; false: not save yet</param>
        void Delete(T entity, bool commitChanges = true);

        /// <summary>
        /// Commits all changes to database.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Gets a table
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        IQueryable<T> TableNoTracking { get; }
    }
}
