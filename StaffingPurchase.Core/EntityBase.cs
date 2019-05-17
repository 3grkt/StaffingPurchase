using System;

namespace StaffingPurchase.Core
{
    public class EntityBase : ICloneable
    {
        /// <summary>
        /// Method executed to get type of entity object.
        /// </summary>
        public static Func<EntityBase, Type> GetObjectTypeMethod = (entity) => { throw new NotImplementedException(); };

        /// <summary>
        /// Gets type of entity object.
        /// </summary>
        /// <returns></returns>
        public Type GetObjectType()
        {
            return GetObjectTypeMethod(this);
        }

        /// <summary>
        /// Clones object.
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return this;
        }
    }
}