using System;
using AI_.Security.Models;


namespace AI_.Security.DAL {
    public interface ISecurityUnitOfWork:IDisposable {
        IRepository<User> UserRepository { get; }
        void Save();
    }
}