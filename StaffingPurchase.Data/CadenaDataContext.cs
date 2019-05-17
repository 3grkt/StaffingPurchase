using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using StaffingPurchase.Core;

namespace StaffingPurchase.Data
{
    public partial class CadenaDataContext : DbContext, IDbContext
    {
        static CadenaDataContext()
        {
            // Register method to get object type
            EntityBase.GetObjectTypeMethod = entity => ObjectContext.GetObjectType(entity.GetType());
        }

        public CadenaDataContext()
            : base("Name=CadenaDataContext")
        {
        }

        #region IDbContext Members

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : EntityBase
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : EntityBase, new()
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

            var result = this.Database.SqlQuery<TEntity>(commandText, parameters).ToList();

            return result;
        }

        public int ExecuteSql(string sql, params object[] parameters)
        {
            return Database.ExecuteSqlCommand(sql, parameters);
        }

        public void DetachEntity<TEntity>(TEntity entity) where TEntity : EntityBase
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginDbTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
