using Abp.DoNetCore.Application;
using Abp.DoNetCore.Application.Dtos;
using Abp.DoNetCore.Common;
using Microsoft.AspNetCore.Mvc;
using MimeoOAWeb.Controllers;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestMimeoOAWeb
{
    public class TestAccount
    {
        [Theory]
        [InlineData("admin", "123456")]
        public async Task TEST_AUTHORIZE(string accountName, string password)
        {
            var mockUser = new ApplicationUser { AccountName = accountName, Password = password };
            Mock<IAbpAuthorizationService> mockAuthorizationService = new Mock<IAbpAuthorizationService>();
            mockAuthorizationService.Setup(item => item.AuthorizationUser(mockUser)).ReturnsAsync(new RESTResult { Code = RESTStatus.Success });
            var authorizeControoler = new AuthorizeController(mockAuthorizationService.Object);
            var result = await authorizeControoler.AuthorizeUser(mockUser);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
