using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace AI_.Security {
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class {
        internal DbContext Context { get; set; }
        internal DbSet<TEntity> DbSet { get; set; }

        public GenericRepository(DbContext context) {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        #region IGenericRepository<TEntity> Members

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "") {
            IQueryable<TEntity> query = DbSet;

            if (filter != null) {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new[] {','}, StringSplitOptions.RemoveEmptyEntries)) {
                query = query.Include(includeProperty);
            }

            if (orderBy != null) {
                return orderBy(query).ToList();
            }
            return query.ToList();
        }

        public virtual TEntity GetByID(object id) {
            return DbSet.Find(id);
        }

        public virtual void Insert(TEntity entity) {
            DbSet.Add(entity);
        }

        public virtual void Delete(object id) {
            TEntity entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete) {
            if (Context.Entry(entityToDelete).State == EntityState.Detached) {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate) {
            DbSet.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        #endregion
    }
}