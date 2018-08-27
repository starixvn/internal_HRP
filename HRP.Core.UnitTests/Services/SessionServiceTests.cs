namespace HRP.Core.UnitTests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using Microsoft.AspNetCore.Http;
    using NSubstitute;
    using Xunit;

    public class SessionServiceTests
    {
        private ISession _session;
        private SessionService sessionService;

        #region UserId

        [Fact]
        public void UserId_SessionServiceWithSession_GetAndSetCorrectly()
        {
            // Arrange
            var userId = 1;
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.UserId = userId;

            // Act
            var actual = sessionService.UserId;

            // Assert
            Assert.Equal(userId, actual);
        }

        [Fact]
        public void UserId_SessionServiceWithHttpContextAccessor_GetAndSetCorrectly()
        {
            // Arrange
            var userId = 1;
            var accessor = Substitute.For<IHttpContextAccessor>();
            accessor.HttpContext = new DefaultHttpContext
            {
                Session = new MockSession()
            };
            sessionService = GetSessionService(accessor);
            sessionService.UserId = userId;

            // Act
            var actual = sessionService.UserId;

            // Assert
            Assert.Equal(userId, actual);
        }

        #endregion UserId

        [Fact]
        public void OnUserId_WhenCalled_GetAndSetCorrectly()
        {
            // Arrange
            var onUserId = 1;
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.OnUserId = onUserId;

            // Act
            var actual = sessionService.OnUserId;

            // Assert
            Assert.Equal(onUserId, actual);
        }

        [Fact]
        public void Username_WhenCalled_GetAndSetCorrectly()
        {
            // Arrange
            var username = "tim.trinh";
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.Username = username;

            // Act
            var actual = sessionService.Username;

            // Assert
            Assert.Equal(username, actual);
        }

        [Fact]
        public void IsSystemAdmin_WhenCalled_GetAndSetCorrectly()
        {
            // Arrange
            var isSystemAdmin = true;
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.IsSystemAdmin = isSystemAdmin;

            // Act
            var actual = sessionService.IsSystemAdmin;

            // Assert
            Assert.Equal(isSystemAdmin, actual);
        }

        [Fact]
        public void MenuItems_WhenCalled_GetAndSetCorrectly()
        {
            // Arrange
            IEnumerable<MenuItem> menuItems = new List<MenuItem>
            {
                new MenuItem { MenuId = 1, Name = "User Management" }
            };
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.MenuItems = menuItems;

            // Act
            var actual = sessionService.MenuItems;

            // Assert
            Assert.Equal(menuItems.Count(), actual.Count());
            Assert.Equal(menuItems.First().MenuId, actual.First().MenuId);
            Assert.Equal(menuItems.First().Name, actual.First().Name);
            Assert.Equal(menuItems.First().Description, actual.First().Description);
            Assert.Equal(menuItems.First().Link, actual.First().Link);
            Assert.Equal(menuItems.First().ParentId, actual.First().ParentId);
            Assert.Equal(menuItems.First().FunctionId, actual.First().FunctionId);
            Assert.Equal(menuItems.First().DisplayOrder, actual.First().DisplayOrder);
            Assert.Equal(menuItems.First().IsActive, actual.First().IsActive);
        }

        [Fact]
        public void Functions_WhenCalled_GetAndSetCorrectly()
        {
            // Arrange
            IEnumerable<FunctionModel> functions = new List<FunctionModel>
            {
                new FunctionModel { FunctionId = 1, FunctionKey = "UserManagement" }
            };
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.Functions = functions;

            // Act
            var actual = sessionService.Functions;

            // Assert
            Assert.Equal(functions.Count(), actual.Count());
            Assert.Equal(functions.First().FunctionId, actual.First().FunctionId);
            Assert.Equal(functions.First().Name, actual.First().Name);
            Assert.Equal(functions.First().FunctionKey, actual.First().FunctionKey);
            Assert.Equal(functions.First().CompanyId, actual.First().CompanyId);
            Assert.Equal(functions.First().ParentId, actual.First().ParentId);
            Assert.Equal(functions.First().IsReadOnly, actual.First().IsReadOnly);
            Assert.Equal(functions.First().IsRead, actual.First().IsRead);
            Assert.Equal(functions.First().IsHost, actual.First().IsHost);
        }

        #region HasPermissionOnFunction

        [Fact]
        public void HasPermissionOnFunction_FunctionKeyIsNotExisted_ReturnFalse()
        {
            // Arrange
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.Functions = new List<FunctionModel>
            {
                new FunctionModel { FunctionId = 1, FunctionKey = "UserManagement" }
            };

            // Act
            var actual = sessionService.HasPermissionOnFunction("test", Arg.Any<Policy>());

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [InlineData(Policy.None)]
        [InlineData(Policy.Read)]
        [InlineData(Policy.Host)]
        public void HasPermissionOnFunction_HasNotPermissionOnFunction_ReturnFalse(Policy policy)
        {
            // Arrange
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.Functions = new List<FunctionModel>
            {
                new FunctionModel { FunctionId = 1, FunctionKey = "UserManagement" }
            };

            // Act
            var actual = sessionService.HasPermissionOnFunction("UserManagement", policy);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void HasPermissionOnFunction_HasReadPermissionOnFunction_ReturnTrue()
        {
            // Arrange
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.Functions = new List<FunctionModel>
            {
                new FunctionModel { FunctionId = 1, FunctionKey = "UserManagement", IsRead = true }
            };

            // Act
            var actual = sessionService.HasPermissionOnFunction("UserManagement", Policy.Read);

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(Policy.Read)]
        [InlineData(Policy.Host)]
        public void HasPermissionOnFunction_HasHostPermissionOnFunction_ReturnTrue(Policy policy)
        {
            // Arrange
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.Functions = new List<FunctionModel>
            {
                new FunctionModel { FunctionId = 1, FunctionKey = "UserManagement", IsHost = true }
            };

            // Act
            var actual = sessionService.HasPermissionOnFunction("UserManagement", policy);

            // Assert
            Assert.True(actual);
        }

        #endregion HasPermissionOnFunction

        #region HasPermissionOnCompany

        [Theory]
        [InlineData(0, "UserManagement")]
        [InlineData(1, null)]
        [InlineData(0, null)]
        public void HasPermissionOnCompany_CompanyIdAndFunctionKeyAreNotExisted_ReturnFalse(int companyId, string functionKey)
        {
            // Arrange
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.Functions = new List<FunctionModel>
            {
                new FunctionModel { FunctionId = 1, CompanyId = 1, FunctionKey = "UserManagement" }
            };

            // Act
            var actual = sessionService.HasPermissionOnCompany(companyId, functionKey, Arg.Any<Policy>());

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [InlineData(Policy.None)]
        [InlineData(Policy.Read)]
        [InlineData(Policy.Host)]
        public void HasPermissionOnCompany_HasNotPermissionOnCompany_ReturnFalse(Policy policy)
        {
            // Arrange
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.Functions = new List<FunctionModel>
            {
                new FunctionModel { FunctionId = 1, CompanyId = 1, FunctionKey = "UserManagement" }
            };

            // Act
            var actual = sessionService.HasPermissionOnCompany(1, "UserManagement", policy);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void HasPermissionOnCompany_HasReadPermissionOnCompany_ReturnTrue()
        {
            // Arrange
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.Functions = new List<FunctionModel>
            {
                new FunctionModel { FunctionId = 1, CompanyId = 1, FunctionKey = "UserManagement", IsRead = true }
            };

            // Act
            var actual = sessionService.HasPermissionOnCompany(1, "UserManagement", Policy.Read);

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(Policy.Read)]
        [InlineData(Policy.Host)]
        public void HasPermissionOnCompany_HasHostPermissionOnCompany_ReturnTrue(Policy policy)
        {
            // Arrange
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.Functions = new List<FunctionModel>
            {
                new FunctionModel { FunctionId = 1, CompanyId = 1, FunctionKey = "UserManagement", IsHost = true }
            };

            // Act
            var actual = sessionService.HasPermissionOnCompany(1, "UserManagement", policy);

            // Assert
            Assert.True(actual);
        }

        #endregion HasPermissionOnCompany

        [Fact]
        public void ClearAll_WhenCalled_ExpectedRemoveAllEntriesInSession()
        {
            // Arrange
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.UserId = 1;

            // Act
            sessionService.ClearAll();

            // Assert
            Assert.Equal(0, sessionService.UserId);
        }

        #region Helpers

        private static SessionService GetSessionService(ISession session)
        {
            var service = new SessionService(session);

            return service;
        }

        private static SessionService GetSessionService(IHttpContextAccessor accessor)
        {
            var service = new SessionService(accessor);

            return service;
        }

        #endregion Helpers
    }
}