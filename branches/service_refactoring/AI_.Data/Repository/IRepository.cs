using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AI_.Data.Repository
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        ICollection<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
                                 Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                 string includeProperties = "");

        ICollection<TEntity> Get(Specification<TEntity> specification);
        TEntity GetByID(object id);
        void Insert(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entityToDelete);
        void Update(TEntity entityToUpdate);
    }
}