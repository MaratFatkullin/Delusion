using System;
using System.Collections.ObjectModel;
using System.Linq;
using AI_.Security.Tests.Mocks;
using AI_.Studmix.Domain.Entities;
using AI_.Studmix.Model.Models;
using AI_.Studmix.Model.Services;
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
    }
}