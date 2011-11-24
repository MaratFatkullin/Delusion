using System.Collections.Generic;

namespace AI_.Data
{
    public class DefaultModelEqualityComparer<TEntity> : IEqualityComparer<TEntity>
        where TEntity : Entity
    {
        public bool Equals(TEntity x, TEntity y)
        {
            return x.ID == y.ID;
        }

        public int GetHashCode(TEntity obj)
        {
            return obj.ID;
        }
    }
}