using AI_.Security.Models;

namespace AI_.Security.DAL {
    public class SecurityUnitOfWork : GenericUnitOfWork<SecurityDbContext>, ISecurityUnitOfWork {
        public GenericRepository<User> UserRepository {
            get { throw new System.NotImplementedException(); }
        }
    }
}