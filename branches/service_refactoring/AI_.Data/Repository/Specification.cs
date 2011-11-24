using System;
using System.Linq;
using System.Linq.Expressions;

namespace AI_.Data.Repository
{
    public abstract class Specification<TEntity>
    {
        public Expression<Func<TEntity, bool>> Filter { get; set; }
        public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> OrderBy { get; set; }
        public string IncludeProperties { get; set; }

        protected Specification()
        {
            Filter = null;
            OrderBy = null;
            IncludeProperties = string.Empty;
        }
    }
}