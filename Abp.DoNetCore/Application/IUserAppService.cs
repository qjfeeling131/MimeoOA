using Abp.DoNetCore.Application.Dtos;
using Abp.DoNetCore.Application.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DoNetCore.Application
{
    public interface IUserAppService
    {
        Task<RESTResult> AddOrUpdateUserAsync(Guid currentUserId, Guid id, bool isAdd, RegisterUserDataObject input);

        Task<RESTResult> AddOrUpdateUserInfo(UserInfoDataTransferObject userInfo, Guid userId);

        Task<bool> UpdateUserAsync(ApplicationUser input);

        Task<UserDataTransferObject> GetUserInformationsAsync(string accountName);

        Task<RESTResult> RemoveUserAsync(Guid id);

        Task<RESTResult> GetUsers(UserDataTransferObject currentUser, int pageIndex, int pageSize);

        Task<RESTResult> GetUserInformationByIdAsync(Guid userId);

        Task<bool> AuthorizationOfUser(ApplicationUser input);

        Task<bool> AddNewRoleAsync(Guid userId, RoleDataTransferObject roleInfo);

        Task<RESTResult> UpdateRoleAsync(Guid currentUserId, bool isDeleted, RoleDataTransferObject roleInfo);

        Task<bool> RemoveRoleAsync(Guid roleId);

        Task<RESTResult> GetRoles(UserDataTransferObject currentUser, int pageIndex, int pageSize);

        Task<RESTResult> GetDepartmentsAsync(UserDataTransferObject currentUser,int pageIndex, int pageSize);

        Task<RESTResult> AddOrUpdateDepartmentAsync(DepartmentDataTransferObject departmentInfo, Guid userId, bool IsDeleted);

        Task<RESTResult> SetDeaprtmentAndRoleAsync(Guid userRoleId, Guid curretnUserId, Guid userId, Guid roleId, Guid departmentId);

        Task<RESTResult> AddOrUpdateUserDepartmentAsync(Guid curretnUserId, Guid userId, Guid departmentId);

        Task<RESTResult> AddOrUpdatePermissionAsync(PermissionTransferDataObject permissionInfo, Guid currentUserId, bool IsDeleted);

        Task<RESTResult> GetPermissionByPagingAsync(Guid currentUserId, int pageIndex, int pageSize);

        Task<RESTResult> SetPermissionToRoleAsync(Guid currentUserId, RolePermissionTransferObject rolePermissionInfo);

        Task<RESTResult> GetMenusToCurrentUser(Guid userId);
    }
}
