using System;
using AI_.Data.Repository;
using AI_.Security.Models;


namespace AI_.Security.DAL {
    public interface ISecurityUnitOfWork:IDisposable {
        IRepository<User> UserRepository { get; }
        IRepository<Role> RoleRepository { get; }
        void Save();
    }
}