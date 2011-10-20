using AI_.Studmix.Model.Services;

namespace AI_.Studmix.Model.Tests.Services
{
    public class PropertyStateServiceTests
    {
        [Xunit.Fact]
        public void GetState_StateExists_StateReturned()
        {
            // Arrange
            var propertyStateService = new PropertyStateService();
            new UnitOfWorkMock
            // Act
            propertyStateService.GetState()

            // Assert
        }
    }
}