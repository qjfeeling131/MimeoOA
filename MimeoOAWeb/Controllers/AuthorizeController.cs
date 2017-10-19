using Abp.DoNetCore;
using Abp.DoNetCore.Application;
using Abp.DoNetCore.Application.Dtos;
using Abp.DoNetCore.Common;
using Abp.DoNetCore.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimeoOAWeb.Controllers
{
    [Route("api/[controller]")]
    
    public class AuthorizeController : BaseController
    {
        private readonly IAbpAuthorizationService authorizationService;
        public AuthorizeController(IAbpAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AuthorizeUser([FromBody]ApplicationUser user)
        {
            return Ok(await this.authorizationService.AuthorizationUser(user));
        }
        
        [HttpGet]
        [Authorize(Policy = MimeoOAPolicyType.PolicyName)]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Write, SystemPermission.Sys_UploadPage)]
        public async Task<IActionResult> AuthroizateUpload()
        {
            return Ok(this.CurrentUser);
        }
    }
}
