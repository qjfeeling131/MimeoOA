using Abp.DoNetCore;
using Abp.DoNetCore.Application;
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
    [Route("api/pages")]
    [Authorize(Policy = MimeoOAPolicyType.PolicyName)]
    public class PageController : BaseController
    {
        private readonly IUserAppService userAppService;

        public PageController(IUserAppService userAppService)
        {
            this.userAppService = userAppService;
        }

        [HttpGet("menu")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Write, SystemPermission.Sys_Read)]
        public async Task<IActionResult> GetPageMenus()
        {
            return Ok(await this.userAppService.GetMenusToCurrentUser(CurrentUser.Id.Value));
        }
    }
}
