using Abp.DoNetCore.Application;
using Abp.DoNetCore.Application.Dtos.Users;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Abp.DoNetCore.Handlers
{
    public class AbpAuthorizationHandler : AuthorizationHandler<JwtUserAhtorizationRequirement>
    {
        private readonly IUserAppService _userService;
        public AbpAuthorizationHandler(IUserAppService userService)
        {
            _userService = userService;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, JwtUserAhtorizationRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier && c.Issuer == "SuperAwesomeTokenServer"))
            {
                return;
            }
            var accountClaim = context.User.FindFirst(c => c.Type == "MimeoUser" && c.Issuer == "SuperAwesomeTokenServer");
            var currentUser = await _userService.GetUserInformationsAsync(accountClaim.Value);
            context.User.AddIdentity(new MimeoOAIdentity(currentUser));
            context.Succeed(requirement);
        }
    }

    public class JwtUserAhtorizationRequirement : IAuthorizationRequirement
    {

    }

    public class MimeoOAIdentity : ClaimsIdentity
    {
        public override string Name => "Admin";
        public override string AuthenticationType => "MimeoOAApplication";
        public UserDataTransferObject CurrentUser { get; }


        public MimeoOAIdentity(UserDataTransferObject currentUser)
        {
            this.CurrentUser = currentUser;
        }

    }
}
