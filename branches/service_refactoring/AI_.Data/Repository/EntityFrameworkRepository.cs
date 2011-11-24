using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace AI_.Data.Repository
{
    public class EntityFrameworkRepository<TEntity> : IRepository<TEntity> 
        where TEntity : Entity
    {
        internal DbContext Context { get; set; }
        internal DbSet<TEntity> DbSet { get; set; }

        public EntityFrameworkRepository(DbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        #region IRepository<TEntity> Members

        public virtual ICollection<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new[] {','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            return query.ToList();
        }

        public virtual ICollection<TEntity> Get(Specification<TEntity> specification)
        {
            return Get(specification.Filter, specification.OrderBy, specification.IncludeProperties);
        }

        public virtual TEntity GetByID(object id)
        {
            return DbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            DbSet.Attach(entityToUpdate);
            entityToUpdate.UpdateDate = DateTime.Now;
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        #endregion
    }
}