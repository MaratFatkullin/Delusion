using AI_.Data;

namespace AI_.Security.Models
{
    public class ModelBase : IIdentifiable<int>
    {
        #region IIdentifiable<int> Members

        public int ID { get; set; }

        #endregion
    }
}