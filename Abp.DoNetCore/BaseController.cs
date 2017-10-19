using Abp.DoNetCore.Application.Dtos.Users;
using Abp.DoNetCore.Filters;
using Abp.DoNetCore.Handlers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace Abp.DoNetCore
{
    [ServiceFilter(typeof(ExceptionFilter))]
    public class BaseController : Controller
    {
        internal protected UserDataTransferObject CurrentUser
        {
            get
            {
                if (this.User == null)
                {
                    return null;
                }
                if (!this.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier && c.Issuer == "SuperAwesomeTokenServer"))
                {
                    return null;
                }
                return (this.User.Identities.Where(item => item.AuthenticationType == "MimeoOAApplication").First() as MimeoOAIdentity).CurrentUser;
            }
        }
        protected string ClientIP { get; }
    }
}
