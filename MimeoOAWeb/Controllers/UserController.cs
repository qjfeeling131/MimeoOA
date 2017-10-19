using Abp.DoNetCore;
using Abp.DoNetCore.Application;
using Abp.DoNetCore.Application.Dtos;
using Abp.DoNetCore.Application.Dtos.Users;
using Abp.DoNetCore.Common;
using Abp.DoNetCore.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MimeoOAWeb.Controllers
{
    [Route("api/users")]
    [Authorize(Policy = MimeoOAPolicyType.PolicyName)]
    public class UserController : BaseController
    {
        private readonly IUserAppService userAppService;
        private readonly IAbpAuthorizationService abpAuthorizationService;
        public UserController(IUserAppService userAppService, IAbpAuthorizationService abpAuthorizationService)
        {
            this.userAppService = userAppService;
            this.abpAuthorizationService = abpAuthorizationService;
            var request = this.Request;
        }

        [HttpPost("user")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Write, SystemPermission.Sys_UserSettingPage)]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserDataObject userInput)
        {
            return Ok(await this.userAppService.AddOrUpdateUserAsync(this.CurrentUser.Id.Value, Guid.Empty, true, userInput));
        }

        [HttpPost("userinfo")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Write, SystemPermission.Sys_UserSettingPage)]
        public async Task<IActionResult> CreateUserInfo(Guid userId, [FromBody] UserInfoDataTransferObject userInfo)
        {
            return Ok(await this.userAppService.AddOrUpdateUserInfo(userInfo, userId));
        }
        [HttpPut("user")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Write, SystemPermission.Sys_UserSettingPage)]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] RegisterUserDataObject userInput)
        {
            return Ok(await this.userAppService.AddOrUpdateUserAsync(this.CurrentUser.Id.Value, id, false, userInput));
        }

        [HttpGet("getCurrentUserInformations")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Read, SystemPermission.Sys_UserSettingPage)]
        public IActionResult GetCurrentUserInformations()
        {
            return Ok(this.CurrentUser);
        }

        [HttpDelete("user")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Write, SystemPermission.Sys_UserSettingPage)]
        public async Task<IActionResult> RemoveUser(Guid userId)
        {
            return Ok(await this.userAppService.RemoveUserAsync(userId));
        }
        [HttpGet("user")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Read, SystemPermission.Sys_UserSettingPage)]
        public async Task<IActionResult> GetUsers(int pageIndex, int pageSize)
        {
            return Ok(await this.userAppService.GetUsers(this.CurrentUser, pageIndex, pageSize));
        }
        [HttpGet("userDetail")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Read, SystemPermission.Sys_UserSettingPage)]
        public async Task<IActionResult> GetUserDetailInformations(Guid id)
        {
            return Ok(await this.userAppService.GetUserInformationByIdAsync(id));
        }

        [HttpPost("role")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Write, SystemPermission.Sys_RoleSettingPage)]
        public async Task<IActionResult> AddRole([FromBody] RoleDataTransferObject roleInfo)
        {
            return Ok(await this.userAppService.AddNewRoleAsync(this.CurrentUser.Id.Value, roleInfo));
        }

        [HttpPut("role")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Write, SystemPermission.Sys_RoleSettingPage)]
        public async Task<IActionResult> UpdateRole([FromBody] RoleDataTransferObject roleInfo)
        {
            return Ok(await this.userAppService.UpdateRoleAsync(CurrentUser.Id.Value, false, roleInfo));
        }

        [HttpDelete("role")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Write, SystemPermission.Sys_RoleSettingPage)]
        public async Task<IActionResult> RemoveRole(Guid id)
        {
            return Ok(await this.userAppService.UpdateRoleAsync(CurrentUser.Id.Value, true, new RoleDataTransferObject { Id = id }));
        }

        [HttpGet("role")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Read, SystemPermission.Sys_RoleSettingPage)]
        public async Task<IActionResult> GetRoles(int pageIndex, int pageSize)
        {
            return Ok(await this.userAppService.GetRoles(CurrentUser, pageIndex, pageSize));
        }
        [HttpGet("department")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Read, SystemPermission.Sys_DepartmentSettingPage)]
        public async Task<IActionResult> GetDepartments(int pageIndex, int pageSize)
        {
            return Ok(await this.userAppService.GetDepartmentsAsync(CurrentUser, pageIndex, pageSize));
        }

        [HttpPost("department")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Write, SystemPermission.Sys_DepartmentSettingPage)]
        public async Task<IActionResult> AddOrUpdateDepartment([FromBody] DepartmentDataTransferObject departmentInfo)
        {
            return Ok(await this.userAppService.AddOrUpdateDepartmentAsync(departmentInfo, CurrentUser.Id.Value, false));
        }

        [HttpDelete("department")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Write, SystemPermission.Sys_DepartmentSettingPage)]
        public async Task<IActionResult> RemoveDepartment(Guid id)
        {
            return Ok(await this.userAppService.AddOrUpdateDepartmentAsync(new DepartmentDataTransferObject { Id = id }, CurrentUser.Id.Value, true));
        }

        [HttpPost("setDepartmentAndRole")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Write, SystemPermission.Sys_ReadAndWrite)]
        public async Task<IActionResult> SetDeaprtmentAndRole([FromBody] UserRoleDataTransferObject departmentRoleInfo)
        {
            return Ok(await this.userAppService.SetDeaprtmentAndRoleAsync(departmentRoleInfo.Id.Value, this.CurrentUser.Id.Value, departmentRoleInfo.UserId, departmentRoleInfo.RoleId, departmentRoleInfo.DepartmentId));
        }
        [HttpPost("userToDepartment")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Write, SystemPermission.Sys_ReadAndWrite)]
        public async Task<IActionResult> SetUserToDepartment([FromBody] UserDepartmentTransferDataObject departmentInfo)
        {
            return Ok(await this.userAppService.AddOrUpdateUserDepartmentAsync(this.CurrentUser.Id.Value, departmentInfo.UserId, departmentInfo.DepartmentId));
        }

        [HttpPost("setRolePermission")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Write, SystemPermission.Sys_ReadAndWrite)]
        public async Task<IActionResult> SetRolePermission([FromBody] RolePermissionTransferObject rolePermissionInfo)
        {
            return Ok(await this.userAppService.SetPermissionToRoleAsync(CurrentUser.Id.Value, rolePermissionInfo));
        }

        [HttpGet("permission")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Read, SystemPermission.Sys_PermissionSettingPage)]
        public async Task<IActionResult> GetPermissions(int pageIndex, int pageSize)
        {
            return Ok(await this.userAppService.GetPermissionByPagingAsync(CurrentUser.Id.Value, pageIndex, pageSize));
        }

        [HttpPost("permission")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Write, SystemPermission.Sys_PermissionSettingPage)]
        public async Task<IActionResult> AddOrUpdatePermission([FromBody]PermissionTransferDataObject permissionInfo)
        {
            return Ok(await this.userAppService.AddOrUpdatePermissionAsync(permissionInfo, CurrentUser.Id.Value, false));
        }

        [HttpDelete("permission")]
        [PermissionFilter(PermissionCondition.And, SystemPermission.Sys_Write, SystemPermission.Sys_PermissionSettingPage)]
        public async Task<IActionResult> RemovePermission(Guid id)
        {
            return Ok(await this.userAppService.AddOrUpdatePermissionAsync(new PermissionTransferDataObject { Id = id }, CurrentUser.Id.Value, true));
        }
    }
}
