using AI_.Studmix.Domain.Entities;

namespace AI_.Studmix.Domain.Factories
{
    public class ContentPackageFactory
    {
        public ContentPackage CreateContentPackage(string caption, string description, User user, int price)
        {
            var contentPackage = new ContentPackage
                                 {
                                     Caption = caption,
                                     Description = description,
                                     Owner = user,
                                     Price = price
                                 };
            return contentPackage;
        }
    }
}