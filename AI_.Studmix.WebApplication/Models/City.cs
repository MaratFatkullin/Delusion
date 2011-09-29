using AI_.Data;

namespace AI_.Studmix.WebApplication.Models
{
    public class City : ModelBase
    {
        public virtual Country Country { get; set; }

        public string Name { get; set; }
    }
}