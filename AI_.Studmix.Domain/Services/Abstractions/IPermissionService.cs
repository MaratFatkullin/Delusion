using AI_.Studmix.Domain.Entities;

namespace AI_.Studmix.Domain.Services.Abstractions
{
    public interface IPermissionService
    {
        bool UserHasPermissions(User user, ContentPackage package);
    }
}