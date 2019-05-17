using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Extensions;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;

namespace StaffingPurchase.Data
{
    /// <summary>
    /// Entity Framework repository
    /// </summary>
    public partial class EfRepository<T> : IRepository<T> where T : EntityBase
    {
        private readonly IDbContext _context;
        private IDbSet<T> _entities;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public EfRepository(IDbContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual T GetById(object id)
        {
            //see some suggested performance optimization (not tested)
            //http://stackoverflow.com/questions/11686225/dbset-find-method-ridiculously-slow-compared-to-singleordefault-on-id/11688189#comment34876113_11688189
            return this.Entities.Find(id);
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="commitChanges">true: save change to db immediately; false: not save yet</param>
        public virtual void Insert(T entity, bool commitChanges = true)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                Entities.Add(entity);

                if (commitChanges)
                {
                    SaveChanges();
                }
            }
            catch (DbUpdateException)
            {
                _context.DetachEntity(entity);
                throw;
            }
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="commitChanges">true: save change to db immediately; false: not save yet</param>
        public virtual void Update(T entity, bool commitChanges = true)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                if (commitChanges)
                {
                    SaveChanges();
                }
            }
            catch (DbUpdateException)
            {
                _context.DetachEntity(entity);
                throw;
            }
        }

        /// <summary>
        /// Bulk update entities
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <param name="updateExpression"></param>
        public virtual int BulkUpdate(Expression<Func<T, bool>> filterExpression,
            Expression<Func<T, T>> updateExpression)
        {
            return _context.Set<T>().Where(filterExpression).Update(updateExpression);
        }

        /// <summary>
        /// Bulk delete entities
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        public virtual int BulkDelete(Expression<Func<T, bool>> filterExpression)
        {
            return _context.Set<T>().Where(filterExpression).Delete();
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="commitChanges">true: save change to db immediately; false: not save yet</param>
        public virtual void Delete(T entity, bool commitChanges = true)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                Entities.Remove(entity);

                if (commitChanges)
                {
                    SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                _context.DetachEntity(entity);

                var baseException = ex.GetBaseException();
                if (baseException is SqlException && ((SqlException)baseException).Number == 547) // key conflict
                {
                    throw new StaffingPurchaseException("Error.DataKeyConflict", ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Commits all changes to database.
        /// </summary>
        public void SaveChanges()
        {
            try
            {
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine +
                               string.Format("Property: {0} Error: {1}", validationError.PropertyName,
                                   validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get { return this.Entities; }
        }


        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public virtual IQueryable<T> TableNoTracking
        {
            get { return this.Entities.AsNoTracking(); }
        }


        /// <summary>
        /// Entities
        /// </summary>
        public virtual IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }
    }
}
