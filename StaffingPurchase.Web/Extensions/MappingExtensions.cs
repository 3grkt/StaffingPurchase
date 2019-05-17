using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using StaffingPurchase.Core;
using StaffingPurchase.Web.Models;

namespace StaffingPurchase.Web.Extensions
{
    public static class MappingExtensions
    {
        #region Convert Entity to Model

        /// <summary>
        /// Converts Entity to Model.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <typeparam name="TModel">Type of model.</typeparam>
        /// <param name="entity">Source entity object.</param>
        /// <returns></returns>
        public static TModel ToModel<TEntity, TModel>(this TEntity entity)
            where TEntity : EntityBase
            where TModel : ViewModelBase
        {
            return Mapper.Map<TEntity, TModel>(entity);
        }

        /// <summary>
        /// Converts Entity to Model.
        /// </summary>
        /// <typeparam name="TModel">Type of model.</typeparam>
        /// <param name="entity">Source entity object.</param>
        /// <param name="destModel">Destination model object.</param>
        /// <returns></returns>
        public static TModel ToModel<TModel>(this EntityBase entity, TModel destModel = null)
            where TModel : ViewModelBase
        {
            if (entity == null)
                return null;

            if (destModel == null)
                return Mapper.Map(entity, entity.GetObjectType(), typeof(TModel)) as TModel;

            return Mapper.Map(entity, destModel, entity.GetObjectType(), typeof(TModel)) as TModel;
        }

        /// <summary>
        /// Converts list of entities to list of models.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="entityList"></param>
        /// <returns></returns>
        public static IList<TModel> ToModelList<TEntity, TModel>(this ICollection<TEntity> entityList)
            where TEntity : EntityBase
            where TModel : ViewModelBase
        {
            return entityList.Select(x => x.ToModel<TModel>()).ToList();
        }

        public static IEnumerable<TModel> ToModelEnumerable<TEntity, TModel>(this IEnumerable<TEntity> entities)
            where TEntity : EntityBase
            where TModel : ViewModelBase
        {
            // TODO: Recheck changes from foreach and yield
            return entities.Select(entity => entity.ToModel<TModel>());
        }

        /// <summary>
        /// Converts list of models to list of entities.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="modelList"></param>
        /// <returns></returns>
        public static IList<TEntity> ToEntityList<TModel, TEntity>(this ICollection<TModel> modelList)
            where TEntity : EntityBase
            where TModel : ViewModelBase
        {
            return modelList.Select(x => x.ToEntity<TEntity>()).ToList();
        }

        #endregion Convert Entity to Model

        #region Convert Model to Entity

        /// <summary>
        /// Converts Model to Entity.
        /// </summary>
        /// <typeparam name="TModel">Type of model.</typeparam>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <param name="model">Source model object.</param>
        /// <returns></returns>
        public static TEntity ToEntity<TModel, TEntity>(this TModel model)
            where TModel : ViewModelBase
            where TEntity : EntityBase
        {
            return Mapper.Map<TModel, TEntity>(model);
        }

        /// <summary>
        /// Converts Model to Entity.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <param name="model">Source model object.</param>
        /// <param name="destEntity">Destination entity object.</param>
        /// <returns></returns>
        public static TEntity ToEntity<TEntity>(this ViewModelBase model, TEntity destEntity = null)
            where TEntity : EntityBase
        {
            if (model == null)
                return null;

            if (destEntity == null)
                return Mapper.Map(model, model.GetType(), typeof(TEntity)) as TEntity;

            return Mapper.Map(model, destEntity, model.GetType(), typeof(TEntity)) as TEntity;
        }

        #endregion Convert Model to Entity

        #region Convert Model to Search Criteria

        /// <summary>
        /// Converts Search Model to Search Criteria.
        /// </summary>
        /// <typeparam name="TSearch">Type of search model.</typeparam>
        /// <typeparam name="TCriteria">Type of search criteria.</typeparam>
        /// <param name="search">Source model object.</param>
        /// <returns></returns>
        public static TCriteria ToCriteria<TSearch, TCriteria>(this TSearch search)
            where TSearch : ViewModelBase
            where TCriteria : SearchCriteriaBase
        {
            return Mapper.Map<TSearch, TCriteria>(search);
        }

        /// <summary>
        /// Converts Search Model to Search Criteria.
        /// </summary>
        /// <typeparam name="TCriteria">Type of search criteria.</typeparam>
        /// <param name="searchModel">Source model object.</param>
        /// <param name="destCriteria">Destination search criteria object.</param>
        /// <returns></returns>
        public static TCriteria ToCriteria<TCriteria>(this ViewModelBase searchModel, TCriteria destCriteria = null)
            where TCriteria : SearchCriteriaBase
        {
            if (searchModel == null)
                return null;

            if (destCriteria == null)
                return Mapper.Map(searchModel, searchModel.GetType(), typeof(TCriteria)) as TCriteria;

            return Mapper.Map(searchModel, destCriteria, searchModel.GetType(), typeof(TCriteria)) as TCriteria;
        }

        #endregion Convert Model to Search Criteria
    }
}
