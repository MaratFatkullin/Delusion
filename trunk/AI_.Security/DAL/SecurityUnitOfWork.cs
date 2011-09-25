using System.Data.Entity;
using AI_.Data.Repository;
using AI_.Security.Models;

namespace AI_.Security.DAL
{
    public abstract class SecurityUnitOfWork<TContext> : UnitOfWork<TContext>, ISecurityUnitOfWork
        where TContext : SecurityDbContext, new()
    {
        private readonly SecurityDbContext _context;
        private IRepository<User> _userRepository;
        private IRepository<Role> _roleRepository;

        public SecurityUnitOfWork()
        {
            _context = new TContext();
        }

        public IRepository<User> UserRepository
        {
            get { return _userRepository ?? (_userRepository = new Repository<User>(_context)); }
        }

        public IRepository<Role> RoleRepository
        {
            get { return _roleRepository ?? (_roleRepository = new Repository<Role>(_context)); }
        }
    }
}