using System;
using System.Linq;
using AI_.Studmix.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AI_.Studmix.Domain.Tests.Enitities
{
    public class PropertyTestsFixture
    {
        protected Property Property;

        public PropertyTestsFixture()
        {
            Property = new Property();
        }
    }


    public class PropertyTests : PropertyTestsFixture
    {
        [Fact]
        public void AddState_StateNotExists_StateAdded()
        {
            // Arrange

            // Act
            Property.AddState("value");

            // Assert
            var propertyState = Property.States.Single();
            propertyState.Value.Should().Be("value");
            propertyState.Index.Should().Be(1);
            propertyState.Property.Should().Be(Property);
        }

        [Fact]
        public void AddState_StateExists_ExceptionThrown()
        {
            // Arrange
            Property.AddState("value");

            // Act
            Property.Invoking(p => p.AddState("value"))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void AddState_ThereIsAnotherStates_IndexHasUniqueValue()
        {
            // Arrange
            Property.AddState("anotherValue");

            // Act
            Property.AddState("value");

            // Assert
            var state = Property.States.Single();
            state.Index.Should().Be(2);
        }
    }
}