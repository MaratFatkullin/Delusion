using AI_.Data;

namespace AI_.Studmix.WebApplication.Models
{
    public class Faculty : ModelBase
    {
        public virtual Institute Institute { get; set; }
        public string Name { get; set; }
    }
}