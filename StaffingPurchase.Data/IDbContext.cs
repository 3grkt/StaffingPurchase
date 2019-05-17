using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using StaffingPurchase.Core;

namespace StaffingPurchase.Data
{
    public interface IDbContext
    {
        IDbSet<TEntity> Set<TEntity>() where TEntity : EntityBase;

        /// <summary>
        /// Executes stored producedure and return list of data in given type;
        /// NOTE: this can be used to execute raw SQL as well.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : EntityBase, new();

        /// <summary>
        /// Executes SQL command and returns number of rows affected.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteSql(string sql, params object[] parameters);

        /// <summary>
        /// Persists changes to database.
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Detaches entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        void DetachEntity<TEntity>(TEntity entity) where TEntity : EntityBase;

        /// <summary>
        /// Starts database transaction.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        IDbTransaction BeginDbTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
    }
}
