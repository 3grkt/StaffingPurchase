using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace StaffingPurchase.Data
{
    public partial class StaffingPurchaseDataContext : DbContext, IDbContext
    {
        static StaffingPurchaseDataContext()
        {
            Database.SetInitializer<StaffingPurchaseDataContext>(null);

            // Register method to get object type
            EntityBase.GetObjectTypeMethod = entity => ObjectContext.GetObjectType(entity.GetType());
        }

        public StaffingPurchaseDataContext()
            : base("Name=StaffingPurchaseDataContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Award> Awards { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<LevelGroup> LevelGroups { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<OrderBatch> OrderBatches { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PackageLog> PackageLogs { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PVLog> PVLogs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Configuration> Configurations { get; set; }

        /// <summary>
        /// Attach an entity to the context or return an already attached entity (if it was already attached)
        /// </summary>
        /// <typeparam name="TEntity">TEntity</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Attached entity</returns>
        protected virtual TEntity AttachEntityToContext<TEntity>(TEntity entity) where TEntity : Core.EntityBase, new()
        {
            ////little hack here until Entity Framework really supports stored procedures
            ////otherwise, navigation properties of loaded entities are not loaded until an entity is attached to the context
            //var alreadyAttached = Set<TEntity>().Local.FirstOrDefault(x => x.Id == entity.Id);
            //if (alreadyAttached == null)
            //{
            //    //attach new entity
            //    Set<TEntity>().Attach(entity);
            //    return entity;
            //}
            //else
            //{
            //    //entity is already loaded.
            //    return alreadyAttached;
            //}

            //Set<TEntity>().Attach(entity);
            return entity;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AwardMap());
            modelBuilder.Configurations.Add(new DepartmentMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new LevelMap());
            modelBuilder.Configurations.Add(new LevelGroupMap());
            modelBuilder.Configurations.Add(new LocationMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new OrderBatchMap());
            modelBuilder.Configurations.Add(new OrderDetailMap());
            modelBuilder.Configurations.Add(new PackageLogMap());
            modelBuilder.Configurations.Add(new PermissionMap());
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new ProductCategoryMap());
            modelBuilder.Configurations.Add(new PVLogMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new StatusMap());
            modelBuilder.Configurations.Add(new PurchaseLimitMap());
            modelBuilder.Configurations.Add(new ConfigurationMap());
        }

        #region IDbContext Members

        public IDbTransaction BeginDbTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            return new DbTransaction(this.Database.BeginTransaction(isolationLevel));
        }

        public void DetachEntity<TEntity>(TEntity entity) where TEntity : Core.EntityBase
        {
            #region Retrieve navigation properties and mark them as 'detached'

            //var type = typeof(TEntity);
            //var collectionProperties = type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).Where(x => x.PropertyType.Name == typeof(ICollection<>).Name);
            //foreach (var collectionProp in collectionProperties)
            //{
            //    this.Entry(entity).Collection(collectionProp.Name).EntityEntry.State = EntityState.Detached;
            //}

            #endregion Retrieve navigation properties and mark them as 'detached'

            if (this.Entry<TEntity>(entity).State == EntityState.Added)
            {
                this.Set<TEntity>().Remove(entity);
            }
            else
            {
                var context = ((IObjectContextAdapter)this).ObjectContext;
                context.Refresh(System.Data.Entity.Core.Objects.RefreshMode.StoreWins, entity);
            }
        }

        public int ExecuteSql(string sql, params object[] parameters)
        {
            return this.Database.ExecuteSqlCommand(sql, parameters);
        }

        /// <summary>
        /// Execute stores procedure and load a list of entities at the end
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Entities</returns>
        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : Core.EntityBase, new()
        {
            commandText = BuildCommandText(commandText, parameters);

            var result = this.Database.SqlQuery<TEntity>(commandText, parameters).ToList();

            //performance hack applied as described here - http://www.nopcommerce.com/boards/t/25483/fix-very-important-speed-improvement.aspx
            bool acd = this.Configuration.AutoDetectChangesEnabled;
            try
            {
                this.Configuration.AutoDetectChangesEnabled = false;

                for (int i = 0; i < result.Count; i++)
                    result[i] = AttachEntityToContext(result[i]);
            }
            finally
            {
                this.Configuration.AutoDetectChangesEnabled = acd;
            }

            return result;
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : Core.EntityBase
        {
            return base.Set<TEntity>();
        }

        private static string BuildCommandText(string commandText, object[] parameters)
        {
            //add parameters to command
            if (parameters != null && parameters.Length > 0)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    var p = parameters[i] as DbParameter;
                    if (p == null)
                        throw new Exception("Not support parameter type");

                    if (p.Value == null)
                        p.Value = DBNull.Value;

                    commandText += i == 0 ? " " : ", ";

                    commandText += "@" + p.ParameterName;
                    if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                    {
                        //output parameter
                        commandText += " output";
                    }
                }
            }
            return commandText;
        }

        #endregion IDbContext Members
    }
}
