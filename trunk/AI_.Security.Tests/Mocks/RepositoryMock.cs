using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AI_.Data.Repository;
using AI_.Security.Models;

namespace AI_.Security.Tests.Mocks
{
    internal class RepositoryMock<TEntity>
        : IRepository<TEntity>
        where TEntity : ModelBase
    {
        private readonly IList<TEntity> _storage;

        public IList<TEntity> Storage
        {
            get { return _storage; }
        }

        public RepositoryMock()
        {
            _storage = new List<TEntity>();
        }

        #region IRepository<TEntity> Members

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
                                        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                        string includeProperties = "")
        {
            IQueryable<TEntity> query = _storage.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            return query.ToList();
        }

        public TEntity GetByID(object id)
        {
            return _storage.Where(entity => entity.ID == (int) id).FirstOrDefault();
        }

        public void Insert(TEntity entity)
        {
            entity.ID = _storage.Count + 1;
            _storage.Add(entity);
        }

        public void Delete(object id)
        {
            _storage.Remove(_storage.First(entity => entity.ID == (int) id));
        }

        public void Delete(TEntity entityToDelete)
        {
            _storage.Remove(entityToDelete);
        }

        public void Update(TEntity entityToUpdate)
        {
            _storage.Remove(_storage.First(entity => entity.ID == entityToUpdate.ID));
            _storage.Add(entityToUpdate);
        }

        #endregion
    }
}