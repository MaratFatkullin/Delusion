using System;
using AI_.Security.Models;


namespace AI_.Security.DAL {
    public interface ISecurityUnitOfWork:IDisposable {
        GenericRepository<User> UserRepository { get; }
        void Save();
    }
}