using System.Collections.ObjectModel;
using AI_.Studmix.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AI_.Studmix.Domain.Tests
{
    public class UserPrincipleTests
    {
        [Fact]
        public void IsInRole_UserInRole_True()
        {
            // Arrange
            var principle = new UserPrinciple();
            principle.Roles.Add(new Role { RoleName = "role" });

            // Act
            var isInRole = principle.IsInRole("role");

            // Assert
            isInRole.Should().BeTrue();
        }

        [Fact]
        public void IsInRole_UserNotInRole_False()
        {
            // Arrange
            var principle = new UserPrinciple();

            // Act
            var isInRole = principle.IsInRole("role");

            // Assert
            isInRole.Should().BeFalse();
        }
    }
}