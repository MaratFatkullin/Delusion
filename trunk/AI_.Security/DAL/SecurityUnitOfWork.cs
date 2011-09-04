using System;
using AI_.Data.Repository;
using AI_.Security.Models;

namespace AI_.Security.DAL
{
    public class SecurityUnitOfWork : UnitOfWork<SecurityDbContext>, ISecurityUnitOfWork
    {
        public IRepository<User> UserRepository
        {
            get { throw new NotImplementedException(); }
        }
    }
}