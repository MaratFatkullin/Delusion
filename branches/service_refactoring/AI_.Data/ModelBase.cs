using System;

namespace AI_.Data
{
    public abstract class ModelBase : IIdentifiable<int>
    {
        #region IIdentifiable<int> Members

        public int ID { get; set; }

        #endregion

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        protected ModelBase()
        {
            CreateDate = DateTime.Now;
        }
    }
}