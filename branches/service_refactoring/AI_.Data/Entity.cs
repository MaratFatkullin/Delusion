using System;

namespace AI_.Data
{
    public abstract class Entity : IIdentifiable<int>
    {
        #region IIdentifiable<int> Members

        public int ID { get; set; }

        #endregion

        public DateTime CreateDate { get; private set; }

        public DateTime? UpdateDate { get; set; }

        protected Entity()
        {
            CreateDate = DateTime.Now;
        }
    }
}