using AI_.Security.DAL;

namespace AI_.Security.Tests.Mocks
{
    public class UnitOfWorkFactoryMock : IUnitOfWorkFactory
    {
        private readonly ISecurityUnitOfWork _unitOfWork;

        public UnitOfWorkFactoryMock()
        {
            _unitOfWork = new SecurityUnitOfWorkMock();
        }

        public ISecurityUnitOfWork GetInstance()
        {
            return _unitOfWork;
        }
    }
}