using Abp.DoNetCore.Handlers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Abp.DoNetCore.Filters
{
    public class PermissionFilter : ActionFilterAttribute
    {
        private readonly string[] sysPermission;
        /// <summary>
        /// Add the condition to filter the permission temporarily, it will optimize the fitler in iteration. 
        /// </summary>
        private readonly PermissionCondition condition;
        public PermissionFilter(PermissionCondition condition, params string[] sysPermission)
        {
            this.sysPermission = sysPermission;
            this.condition = condition;
        }
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContextUser = context.HttpContext.User;
            if (httpContextUser == null)
            {
                return null;
            }
            if (!httpContextUser.HasClaim(c => c.Type == ClaimTypes.NameIdentifier && c.Issuer == "SuperAwesomeTokenServer"))
            {
                return null;
            }
            var currentUser = (httpContextUser.Identities.Where(item => item.AuthenticationType == "MimeoOAApplication").First() as MimeoOAIdentity).CurrentUser;
            var permissions = currentUser.Permissions.Where(item => sysPermission.Contains(item.Id.ToString()));
            if (this.condition.Equals(PermissionCondition.And))
            {
                if (!permissions.Count().Equals(sysPermission.Count()))
                {
                    throw new ArgumentException("Don't have the permssion to visit this method");
                }
            }
            else
            {
                if (permissions.Count() <= 0)
                {
                    throw new ArgumentException("Don't have the permssion to visit this method");
                }
            }
            return base.OnActionExecutionAsync(context, next);
        }
    }

    public enum PermissionCondition
    {
        And,
        Or
    }
}
