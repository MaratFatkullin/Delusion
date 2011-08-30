using System.Web.Security;
using AI_.Security.DAL;
using AI_.Security.Models;
using AI_.Security.Providers;
using Xunit;
using Moq;

namespace AI_.Security.Tests.Providers {
    public class CustomMembershipProviderFixtrure {
        private readonly CustomMembershipProvider _provider;

        public CustomMembershipProviderFixtrure() {
            var mock = new Mock<ISecurityUnitOfWork>();
            var mock1 = new Mock<IGenericRepository<User>>();
            //mock1.Setup(instance => instance.)
            //mock.Setup(instance => instance.)
            _provider = new CustomMembershipProvider();
        }

        [Fact]
        public void fact1() {
            MembershipCreateStatus status;
            _provider.CreateUser("username",
                                 "password",
                                 "email",
                                 "passwordQuestion",
                                 "passwordAnswer",
                                 true, 
                                 null, 
                                 out status);
        }
    }
}