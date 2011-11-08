using System;
using System.Collections.ObjectModel;
using System.Linq;
using AI_.Studmix.Model.Models;
using AI_.Studmix.Model.Services;
using AI_.Studmix.WebApplication.Tests.Mocks;
using FluentAssertions;
using Xunit;

namespace AI_.Studmix.WebApplication.Tests.Model.Services
{
    public class PropertyStateServiceTests
    {
        #region Utility methods

        private static Property CreateProperty(int id = 1)
        {
            return new Property {ID = id, States = new Collection<PropertyState>()};
        }

        private static PropertyState CreatePropertyState(Property property)
        {
            var propertyState = new PropertyState {Property = property, Value = "value"};
            property.States.Add(propertyState);
            return propertyState;
        }

        #endregion

        [Fact]
        public void GetState_StateExists_StateReturned()
        {
            // Arrange
            var unitOfWork = new UnitOfWorkMock();
            var propertyStateService = new PropertyStateService(unitOfWork);
            var property = CreateProperty();
            var propertyState = CreatePropertyState(property);
            unitOfWork.GetRepository<PropertyState>().Insert(propertyState);
            unitOfWork.Save();

            // Act
            var state = propertyStateService.GetState(property.ID, propertyState.Value);

            // Assert
            state.Should().Be(propertyState);
        }

        [Fact]
        public void GetState_StateNotExists_NullReturned()
        {
            // Arrange
            var unitOfWork = new UnitOfWorkMock();
            var propertyStateService = new PropertyStateService(unitOfWork);
            var property = CreateProperty();

            // Act
            var state = propertyStateService.GetState(property.ID, "value");

            // Assert
            state.Should().BeNull();
        }

        [Fact]
        public void GetState_PropertyNotExists_NullReturned()
        {
            // Arrange
            var unitOfWork = new UnitOfWorkMock();
            var propertyStateService = new PropertyStateService(unitOfWork);
            var property = CreateProperty();
            var propertyState = CreatePropertyState(property);
            unitOfWork.GetRepository<PropertyState>().Insert(propertyState);

            // Act
            var state = propertyStateService.GetState(2, "value");

            // Assert
            state.Should().BeNull();
        }

        [Fact]
        public void CreatePropertyState_StateNotExists_StateCreated()
        {
            // Arrange
            var unitOfWork = new UnitOfWorkMock();
            var service = new PropertyStateService(unitOfWork);
            var property = CreateProperty();
            unitOfWork.GetRepository<Property>().Insert(property);
            unitOfWork.GetRepository<Property>().Insert(property);
            unitOfWork.Save();

            // Act
            service.CreateState(property, "newValue");

            // Assert
            unitOfWork.GetRepository<PropertyState>().Get().Should().Contain(x => x.Value == "newValue");
        }


        [Fact]
        public void CreatePropertyState_StateNotExists_NewStateReturned()
        {
            // Arrange
            var unitOfWork = new UnitOfWorkMock();
            var service = new PropertyStateService(unitOfWork);
            var property = CreateProperty();
            unitOfWork.GetRepository<Property>().Insert(property);
            unitOfWork.GetRepository<Property>().Insert(property);
            unitOfWork.Save();

            // Act
            var newState = service.CreateState(property, "newValue");

            // Assert
            newState.Should().Be(unitOfWork.GetRepository<PropertyState>().Get().Last());
        }


        [Fact]
        public void CreatePropertyState_StateExists_ExceptionThrown()
        {
            // Arrange
            var unitOfWork = new UnitOfWorkMock();
            var service = new PropertyStateService(unitOfWork);
            var property = CreateProperty();
            var propertyState = CreatePropertyState(property);
            unitOfWork.GetRepository<PropertyState>().Insert(propertyState);

            // Act, Assert
            service.Invoking(srv => srv.CreateState(property, propertyState.Value))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void CreatePropertyState_StateOfSamePropertyNotExists_StateHasUniquePropertyStateIndex()
        {
            // Arrange
            var unitOfWork = new UnitOfWorkMock();
            var service = new PropertyStateService(unitOfWork);
            var propertyState = CreatePropertyState(CreateProperty(id:1));
            unitOfWork.GetRepository<PropertyState>().Insert(propertyState);
            unitOfWork.Save();

            // Act
            service.CreateState(CreateProperty(id:2), "newValue");

            // Assert
            unitOfWork.GetRepository<PropertyState>().Get().Should().Contain(x => x.Index == 1);
        }

        [Fact]
        public void CreatePropertyState_StateOfSamePropertyExists_StateHasUniquePropertyStateIndex()
        {
            // Arrange
            var unitOfWork = new UnitOfWorkMock();
            var service = new PropertyStateService(unitOfWork);
            var property = CreateProperty();
            var propertyState = CreatePropertyState(property);
            propertyState.Index = 2;
            unitOfWork.GetRepository<PropertyState>().Insert(propertyState);
            unitOfWork.Save();

            // Act
            service.CreateState(property, "newValue");

            // Assert
            unitOfWork.GetRepository<PropertyState>().Get().Last().Index.Should().Be(3);
        }
    }
}