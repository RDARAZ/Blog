using Blog.Domain.Entities;
using Blog.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using FluentAssertions;
using Blog.Application.Interfaces;

namespace Blog.Infrastructure.Tests.Services;

public class UserRoleServiceTests
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly IUserRoleService _userRoleService;
    private readonly User _testUser;

    public UserRoleServiceTests()
    {
        var mockStore = new Mock<IUserStore<User>>();
        _mockUserManager = new Mock<UserManager<User>>(
            mockStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        _userRoleService = new UserRoleService(_mockUserManager.Object);

        _testUser = new User
        {
            Id = 1,
            UserName = "testuser",
            Email = "test@example.com",
            IsActive = true
        };
    }

    [Fact]
    public async Task IsAdminAsync_WhenUserIsAdmin_ShouldReturnTrue()
    {
        //Arrange
        _mockUserManager.Setup(x => x.IsInRoleAsync(_testUser, Role.RoleName.Admin))
            .ReturnsAsync(true);
        //Act
        var result = await _userRoleService.IsAdminAsync(_testUser);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanWriteArticlesAsync_WhenUserIsActiveAdmin_ShouldReturnTrue()
    {
        // Arrange
        _testUser.IsActive = true;
        _mockUserManager.Setup(x => x.IsInRoleAsync(_testUser, Role.RoleName.Admin))
            .ReturnsAsync(true);

        // Act
        var result = await _userRoleService.CanWriteArticlesAsync(_testUser);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanWriteArticlesAsync_WhenUserIsInactive_ShouldReturnFalse()
    {
        // Arrange
        _testUser.IsActive = false;
        _mockUserManager.Setup(x => x.IsInRoleAsync(_testUser, Role.RoleName.Admin))
            .ReturnsAsync(true);

        // Act
        var result = await _userRoleService.CanWriteArticlesAsync(_testUser);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddUserToRoleAsync_WhenSuccessful_ShouldReturnTrue()
    {
        // Arrange
        _mockUserManager.Setup(x => x.AddToRoleAsync(_testUser, Role.RoleName.User))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userRoleService.AddUserToRoleAsync(_testUser, Role.RoleName.User);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetUserRolesAsync_ShouldReturnUserRoles()
    {
        // Arrange
        var expectedRoles = new List<string> { Role.RoleName.User };
        _mockUserManager.Setup(x => x.GetRolesAsync(_testUser))
            .ReturnsAsync(expectedRoles);

        // Act
        var result = await _userRoleService.GetUserRolesAsync(_testUser);

        // Assert
        result.Should().BeEquivalentTo(expectedRoles);
    }
}
