using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.DoNetCore.Application.Dtos;
using Abp.DoNetCore.Application.Dtos.Users;
using Abp.DoNetCore.Common;
using Abp.DoNetCore.Domain;
using Abp.DoNetCore.Domain.XmlObjects;
using Abp.Runtime.Caching;
using Abp.Utilities;
using AutoMapper;
using ExtendedXmlSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.DoNetCore.Application
{
    public class UserAppService : DomainService, IUserAppService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserInfo> _userInfoRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<RolePermission> _rolePermissionRepository;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IRepository<Department> _departmentReposiotry;
        private readonly IRepository<UserDepartment> _userDepartmentReposiotry;
        private readonly ICache _redisCache;
        private readonly IExtendedXmlSerializer _serializer;
        public UserAppService(IRepository<User> userRepository, IRepository<UserInfo> userInfoRepository, IRepository<Role> roleRepository, IRepository<UserRole> userRoleRepository, IRepository<RolePermission> rolePermissionRepository, IRepository<Permission> permissionRepository, IRepository<Department> departmentReposiotry, IRepository<UserDepartment> userDepartmentReposiotry, ICache cache, IExtendedXmlSerializer serializer)
        {
            _userRepository = userRepository;
            _userInfoRepository = userInfoRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _permissionRepository = permissionRepository;
            _departmentReposiotry = departmentReposiotry;
            _userDepartmentReposiotry = userDepartmentReposiotry;
            _redisCache = cache;
            _serializer = serializer;
        }

        #region User
        public async Task<RESTResult> AddOrUpdateUserAsync(Guid currentUserId, Guid id, bool isAdd, RegisterUserDataObject input)
        {
            RESTResult result = new RESTResult { Code = RESTStatus.Failed };
            if (!RegUtility.IsValidationForEmail(input.AccountEmail))
            {
                throw new ArgumentException("Wrong Email format!");
            }
            if (!string.IsNullOrEmpty(input.AccountPhone) && !RegUtility.IsValidationForPhone(input.AccountPhone))
            {
                throw new ArgumentException("Wrong Phone format!");
            }
            if (isAdd)
            {
                //Check the user whteher exist in User
                var userEntities = await _userRepository.GetAllListAsync(item => item.AccountEmail.Equals(input.AccountEmail) || item.AccountCode.Equals(input.AccountCode) || item.AccountPhone.Equals(input.AccountPhone));
                if (userEntities.Count > 0)
                {
                    throw new ArgumentException($"The user {input.AccountEmail} {input.AccountCode} {input.AccountEmail} have been exist");
                }
                var userEntity = new User();
                userEntity.Id = Guid.NewGuid();
                userEntity.AccountEmail = input.AccountEmail;
                userEntity.AccountPhone = input.AccountPhone;
                userEntity.AccountCode = input.AccountCode;
                //It need to be optimized.
                userEntity.LastLoginIP = "127.0.0.1";
                userEntity.CreateTime = DateTime.Now;
                userEntity.LastLoginTime = DateTime.Now;
                userEntity.Password = HashUtility.CreateHash(string.IsNullOrEmpty(input.Password) ? GeneralPassword(input.AccountEmail) : input.Password);
                await _userRepository.InsertAsync(userEntity);
                if (input.UserInfo != null)
                {
                    var userInfoEntity = Mapper.Map<UserInfoDataTransferObject, UserInfo>(input.UserInfo);
                    await _userInfoRepository.InsertAsync(userInfoEntity);
                }
                await SetUserAndDepartmentMapAsync(currentUserId, userEntity.Id, input.DepartmentId);
                await SetUserAndRoleMapAsync(currentUserId, userEntity.Id, input.RoleIds);
                result.Code = RESTStatus.Success;
                result.Message = "Add user successful";
            }
            else
            {
                if (id == null || Guid.Empty.Equals(id))
                {
                    throw new ArgumentException("The user id can not be null");
                }
                //TODO:Update the User
                var userEntity = await _userRepository.GetAsync(id);
                if (userEntity == null)
                {
                    result.Message = $"The user(id is {id}) doesn't exist";
                    return result;
                }
                userEntity.AccountEmail = input.AccountEmail;
                userEntity.AccountPhone = input.AccountPhone;
                userEntity.ModifyByUserId = currentUserId;
                userEntity.ModifyTime = DateTime.Now;
                await _userRepository.UpdateAsync(userEntity);
                //TODO:Update the user info;
                var userInfoEntity = (await _userInfoRepository.GetAllListAsync(item => item.UserId.Equals(id))).FirstOrDefault();
                if (userInfoEntity != null)
                {
                    var userInfoModel = Mapper.Map<UserInfoDataTransferObject, UserInfo>(input.UserInfo);
                    userInfoModel.Id = userInfoEntity.Id;
                    userInfoModel.UserId = id;
                    await _userInfoRepository.UpdateAsync(userInfoModel);
                }
                else
                {
                    //TODO:Add new Informations
                    var userinfoModel = Mapper.Map<UserInfoDataTransferObject, UserInfo>(input.UserInfo);
                    userinfoModel.UserId = userEntity.Id;
                    await _userInfoRepository.InsertAsync(userinfoModel);
                }
                //
                //TODO:Update the department;
                await SetUserAndDepartmentMapAsync(currentUserId, userEntity.Id, input.DepartmentId);
                //TODO:Remove before user role;
                await RemoveBeforeUserRole(userEntity.Id);
                //TODO:update the role;
                await SetUserAndRoleMapAsync(currentUserId, userEntity.Id, input.RoleIds);
                result.Code = RESTStatus.Success;
                result.Message = "update user successful";
            }

            return result;
        }

        [UnitOfWork(IsDisabled = true)]
        /// <summary>
        /// It should include the base user informations(inlcude role, permission and so on)
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public async Task<UserDataTransferObject> GetUserInformationsAsync(string accountName)
        {
            var userModel = await GetUserByAccountName(accountName);
            var userDto = Mapper.Map<User, UserDataTransferObject>(userModel);
            var departmentModel = await GetCurrentUserDepartment(userModel.Id);
            userDto.Department = departmentModel != null ? Mapper.Map<Department, DepartmentDataTransferObject>(departmentModel) : null;
            var roleModels = departmentModel != null ? await GetRolesWithCurrentUser(userModel.Id, departmentModel.Id) : await GetRolesWithCurrentUser(userModel.Id, Guid.Empty);
            foreach (var item in roleModels)
            {
                userDto.Roles.Add(Mapper.Map<Role, RoleDataTransferObject>(item));
            }
            var permissionModels = await GetCurrentUserPermissions(userModel.Id);
            foreach (var item in permissionModels)
            {
                userDto.Permissions.Add(Mapper.Map<Permission, PermissionTransferDataObject>(item));
            }
            return userDto;
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser input)
        {
            var userModel = await GetUserByAccountName(input.AccountName);
            var result = await _userRepository.UpdateAsync(userModel);
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public async Task<RESTResult> RemoveUserAsync(Guid id)
        {
            RESTResult result = new RESTResult { Code = RESTStatus.Success };
            var userModel = await _userRepository.UpdateAsync(id, item => { item.IsDeleted = true; return _userRepository.UpdateAsync(item); });
            result.Data = Mapper.Map<User, ApplicationUser>(userModel);
            return result;
        }
        [UnitOfWork(IsDisabled = true)]
        public async Task<RESTResult> GetUsers(UserDataTransferObject currentUser, int pageIndex, int pageSize)
        {
            RESTResult result = new RESTResult { Code = RESTStatus.Success };
            List<UserDataTransferObject> userDTOs = new List<UserDataTransferObject>();

            //TODO: Super admin
            if (currentUser.Roles.Where(item => item.Level.Equals(RoleLevelStatus.SupperAdmin)).Count() > 0)
            {
                var allUserCount = await _userRepository.CountAsync();
                var allUser = (await _userRepository.GetAllListAsync()).Take(pageIndex * pageSize).Skip(pageSize * (pageIndex - 1));
                allUser.ToList().ForEach(item => userDTOs.Add(Mapper.Map<User, UserDataTransferObject>(item)));
                result.Data = new { users = userDTOs, Count = allUserCount };
                return result;
            }
            if (currentUser.Department == null || currentUser.Permissions == null)
            {
                result.Code = RESTStatus.NotData;
                return result;
            }
            var allUserIds = await _userDepartmentReposiotry.GetAllListAsync(item => item.DepartmentId.Equals(currentUser.Department.Id));
            var userModels = _userRepository.GetAllList(item => allUserIds.Select(c => c.UserId).Contains(item.Id) && item.IsDeleted.Equals(false)).Take(pageIndex * pageSize).Skip(pageSize * (pageIndex - 1));
            //TODO:Mapping, DOT NOT USE THE .ToList()
            foreach (var item in userModels)
            {
                userDTOs.Add(Mapper.Map<User, UserDataTransferObject>(item));
            }
            var userCount = await _userRepository.CountAsync(item => item.IsDeleted.Equals(false));
            result.Data = new { users = userDTOs, Count = userCount };
            return result;
        }

        [UnitOfWork(IsDisabled = true)]
        public async Task<RESTResult> GetUserInformationByIdAsync(Guid userId)
        {
            RESTResult result = new RESTResult { Code = RESTStatus.Success };
            if (userId == null && Guid.Empty.Equals(userId))
            {
                throw new ArgumentException("the user id can not be null");
            }
            var userModel = await _userRepository.GetAsync(userId);
            if (userModel.IsDeleted)
            {
                throw new ArgumentException($"the current user{userModel.AccountCode} have been removed");
            }
            var userDataObject = Mapper.Map<User, UserDataTransferObject>(userModel);
            var departmentModel = await GetCurrentUserDepartment(userModel.Id);
            var userInfoModel = (await _userInfoRepository.GetAllListAsync(item => item.UserId.Equals(userId))).FirstOrDefault();
            if (userInfoModel != null)
            {
                userDataObject.UserInfo = Mapper.Map<UserInfo, UserInfoDataTransferObject>(userInfoModel);
            }
            userDataObject.Department = departmentModel != null ? Mapper.Map<Department, DepartmentDataTransferObject>(departmentModel) : null;
            var roleModels = departmentModel != null ? await GetRolesWithCurrentUser(userModel.Id, departmentModel.Id) : await GetRolesWithCurrentUser(userModel.Id, Guid.Empty);
            foreach (var item in roleModels)
            {
                userDataObject.Roles.Add(Mapper.Map<Role, RoleDataTransferObject>(item));
            }
            var permissionModels = await GetCurrentUserPermissions(userModel.Id);
            foreach (var item in permissionModels)
            {
                userDataObject.Permissions.Add(Mapper.Map<Permission, PermissionTransferDataObject>(item));
            }
            result.Data = userDataObject;
            return result;
        }

        public async Task<RESTResult> AddOrUpdateUserInfo(UserInfoDataTransferObject userInfo, Guid userId)
        {
            RESTResult result = new RESTResult { Code = RESTStatus.Failed };
            var userInfos = await _userInfoRepository.GetAllListAsync(item => item.UserId.Equals(userId));
            if (userInfos.Count <= 0)
            {
                //TODO:Add user info
                var userInfoModle = Mapper.Map<UserInfoDataTransferObject, UserInfo>(userInfo);
                userInfoModle.UserId = userId;
                await _userInfoRepository.InsertAsync(userInfoModle);
                result.Code = RESTStatus.Success;
                result.Message = "Create User info successful";
                return result;
            }
            else
            {
                //TODO:Update user info
                var userinfoEntity = userInfos.First();
                userinfoEntity.FirstName = userInfo.FirstName;
                userinfoEntity.LastName = userInfo.LastName;
                userinfoEntity.NickName = userInfo.NickName;
                userinfoEntity.CardType = userInfo.CardType;
                userinfoEntity.CardNo = userInfo.CardNo;
                userinfoEntity.Phone = userInfo.Phone;
                userinfoEntity.Tel = userInfo.Tel;
                userinfoEntity.Address = userInfo.Address;
                userinfoEntity.BirthDate = userInfo.BirthDate;
                userinfoEntity.ExtendInfo = userInfo.ExtendInfo;
                await _userInfoRepository.UpdateAsync(userinfoEntity);
                result.Code = RESTStatus.Success;
                result.Data = userInfo;
            }
            return result;
        }

        [UnitOfWork(IsDisabled = true)]
        /// <summary>
        /// It should be returned this current user entity
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> AuthorizationOfUser(ApplicationUser input)
        {
            var haveLoginUser = await GetUserByAccountName(input.AccountName);
            if (haveLoginUser == null)
            {
                return false;
            }
            //validate the password
            var password = haveLoginUser.Password;
            return HashUtility.ValidatePassword(input.Password, password);
        }
        #endregion

        #region Department

        [UnitOfWork(IsDisabled = true)]
        public async Task<RESTResult> GetDepartmentsAsync(UserDataTransferObject currentUser, int pageIndex, int pageSize)
        {
            RESTResult result = new RESTResult { Code = RESTStatus.Success };
            if (currentUser.Roles.Where(item => item.Level.Equals(RoleLevelStatus.SupperAdmin)).Count() > 0)
            {
                var departmentList = await _departmentReposiotry.GetAllListAsync(item => item.IsDeleted.Equals(false));
                var pagingDepartmentList = departmentList.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
                result.Data = pagingDepartmentList.Select(item =>
                {
                    var departmentDTO = Mapper.Map<Department, DepartmentDataTransferObject>(item);
                    return departmentDTO;
                });
            }
            else
            {
                if (currentUser.Department != null)
                {
                    var departmentModel = await _departmentReposiotry.GetAllListAsync(item => item.Id.Equals(currentUser.Department.Id) && item.IsDeleted.Equals(false));
                    result.Data = departmentModel.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
                }
                else
                {
                    result.Code = RESTStatus.NotData;
                }

            }
            return result;
        }

        public async Task<RESTResult> AddOrUpdateDepartmentAsync(DepartmentDataTransferObject departmentInfo, Guid userId, bool IsDeleted)
        {
            RESTResult result = new RESTResult { Code = RESTStatus.Failed };
            if (Guid.Empty.Equals(departmentInfo.Id) || departmentInfo.Id == null)
            {
                //TODO:Add a new department
                var deparmentEntity = Mapper.Map<DepartmentDataTransferObject, Department>(departmentInfo);
                deparmentEntity.CreateTime = DateTime.Now;
                deparmentEntity.CreateByUserId = userId;
                await _departmentReposiotry.InsertAsync(deparmentEntity);
                result.Code = RESTStatus.Success;
                result.Message = "Add the department successful";

            }
            else
            {
                //TODO:Update department
                var departmentEntity = await _departmentReposiotry.GetAsync(departmentInfo.Id.Value);
                if (departmentEntity.IsDeleted)
                {
                    result.Message = "The department have been removed";
                    return result;
                }
                if (departmentEntity != null)
                {
                    departmentEntity.Name = string.IsNullOrEmpty(departmentInfo.Name) ? departmentEntity.Name : departmentInfo.Name;
                    departmentEntity.ModifyByUserId = userId;
                    departmentEntity.ModifyTime = DateTime.Now;
                    departmentEntity.IsDeleted = IsDeleted;
                    await _departmentReposiotry.UpdateAsync(departmentEntity);
                    result.Code = RESTStatus.Success;
                    result.Message = "update the department successful";
                }
            }
            return result;
        }

        public async Task<RESTResult> SetDeaprtmentAndRoleAsync(Guid userRoleId, Guid curretnUserId, Guid userId, Guid roleId, Guid departmentId)
        {
            RESTResult result = new RESTResult
            {
                Code = RESTStatus.Success
            };
            UserRole userRoleEntity;
            if (Guid.Empty.Equals(userRoleId))
            {
                //TODO:Add new user role
                userRoleEntity = new UserRole
                {
                    RoleId = roleId,
                    UserId = userId,
                    CreateByUserId = curretnUserId,
                    CreateTime = DateTime.Now
                };
                await _userRoleRepository.InsertAsync(userRoleEntity);
            }
            else
            {
                userRoleEntity = await _userRoleRepository.GetAsync(userRoleId);
                userRoleEntity.RoleId = roleId;
                userRoleEntity.ModifyByUserId = curretnUserId;
                userRoleEntity.ModifyTime = DateTime.Now;
                await _userRoleRepository.UpdateAsync(userRoleEntity);
            }
            if (userRoleEntity != null)
            {
                result.Message = "Set departmnet and role successful";
                return result;
            }

            result.Code = RESTStatus.Failed;
            result.Message = "Set departmnet and role failed";
            return result;

        }

        public async Task<RESTResult> AddOrUpdateUserDepartmentAsync(Guid curretnUserId, Guid userId, Guid departmentId)
        {
            RESTResult result = new RESTResult
            {
                Code = RESTStatus.Success
            };
            var userDepartments = await _userDepartmentReposiotry.GetAllListAsync(item => item.UserId.Equals(userId));
            if (userDepartments.Count > 0)
            {
                //We should not allow the user have two department at the smae time temporarily
                var userDepartmentModel = userDepartments.FirstOrDefault();
                userDepartmentModel.DepartmentId = departmentId;
                userDepartmentModel.ModifyByUserId = curretnUserId;
                userDepartmentModel.ModifyTime = DateTime.Now;
                await _userDepartmentReposiotry.UpdateAsync(userDepartmentModel);
                result.Message = "Set department successful";
            }
            else
            {
                //TODO:Add new user role
                var userDepartmentEntity = new UserDepartment
                {
                    UserId = userId,
                    DepartmentId = departmentId,
                    CreateByUserId = curretnUserId,
                    CreateTime = DateTime.Now
                };
                await _userDepartmentReposiotry.InsertAsync(userDepartmentEntity);
                result.Message = "Set department successful";
            }
            return result;
        }



        #endregion

        #region Role

        public async Task<RESTResult> UpdateRoleAsync(Guid currentUserId, bool isDeleted, RoleDataTransferObject roleInfo)
        {
            RESTResult result = new RESTResult { Code = RESTStatus.Failed };
            var roleEntities = await _roleRepository.GetAllListAsync(item => item.Id == roleInfo.Id.Value && item.IsDeleted.Equals(false));
            if (roleEntities.Count > 0)
            {
                var roleEntity = roleEntities.First();
                if (roleEntity.Level.Equals(RoleLevelStatus.SupperAdmin))
                {
                    throw new ArgumentException($"This role {roleInfo.Name} can not be changes");
                }
                roleEntity.Name = string.IsNullOrEmpty(roleInfo.Name) ? roleEntity.Name : roleInfo.Name;
                roleEntity.ModifyByUserId = currentUserId;
                roleEntity.ModifyTime = DateTime.Now;
                roleEntity.IsDeleted = isDeleted;
                roleEntity.Level = RoleLevelStatus.Other;
                var userModel = await _roleRepository.UpdateAsync(roleEntity);
                result.Code = RESTStatus.Success;
                result.Data = Mapper.Map<Role, RoleDataTransferObject>(userModel);
                return result;
            }
            return result;
        }

        public async Task<bool> AddNewRoleAsync(Guid userId, RoleDataTransferObject roleInfo)
        {
            var roleNewEntity = Mapper.Map<RoleDataTransferObject, Role>(roleInfo);
            roleNewEntity.CreateByUserId = userId;
            roleNewEntity.CreateTime = DateTime.Now;
            roleNewEntity.Code = userId.ToString();
            roleNewEntity.IsDeleted = false;
            roleNewEntity.Level = RoleLevelStatus.Other;
            var result = await _roleRepository.InsertAsync(roleNewEntity);
            if (result != null)
            {
                return true;

            }
            return false;
        }

        public async Task<RESTResult> GetRoles(UserDataTransferObject currentUser, int pageIndex, int pageSize)
        {
            RESTResult result = new RESTResult { Code = RESTStatus.Success };
            List<RoleDataTransferObject> rolesDataobjects = new List<RoleDataTransferObject>();
            if (currentUser.Roles.Where(item => item.Level.Equals(RoleLevelStatus.SupperAdmin)).Count() > 0)
            {
                var roles = _roleRepository.GetAllList(item => item.IsDeleted.Equals(false) && !item.Level.Equals(RoleLevelStatus.SupperAdmin)).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
                foreach (var role in roles)
                {
                    //TODO:Get role permissions
                    var roleDataObject = Mapper.Map<Role, RoleDataTransferObject>(role);
                    var rolePermissions = await _rolePermissionRepository.GetAllListAsync(item => role.Id.Equals(item.RoleId));
                    var permissions = await _permissionRepository.GetAllListAsync(item => rolePermissions.Select(c => c.PermissionId).Contains(item.Id));
                    permissions.ForEach(item => roleDataObject.Permissions.Add(Mapper.Map<Permission, PermissionTransferDataObject>(item)));
                    rolesDataobjects.Add(roleDataObject);
                }
                result.Data = rolesDataobjects;
            }
            else
            {
                //Get curretnt role under the department.
                var roles = (await GetRolesWithCurrentUser(currentUser.Id.Value, currentUser.Department.Id.Value)).Where(c => c.Level.Equals(RoleLevelStatus.Other)).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
                foreach (var role in roles)
                {
                    //TODO:Get role permissions
                    var roleDataObject = Mapper.Map<Role, RoleDataTransferObject>(role);
                    var rolePermissions = await _rolePermissionRepository.GetAllListAsync(item => role.Id.Equals(item.RoleId));
                    var permissions = await _permissionRepository.GetAllListAsync(item => rolePermissions.Select(c => c.PermissionId).Contains(item.Id));
                    permissions.ForEach(item => roleDataObject.Permissions.Add(Mapper.Map<Permission, PermissionTransferDataObject>(item)));
                    rolesDataobjects.Add(roleDataObject);
                }
                result.Data = rolesDataobjects;
            }

            return result;
        }

        public async Task<bool> RemoveRoleAsync(Guid roleId)
        {
            var result = await _roleRepository.UpdateAsync(roleId, item => { item.IsDeleted = true; return _roleRepository.UpdateAsync(item); });
            if (result != null)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Permission
        public async Task<RESTResult> AddOrUpdatePermissionAsync(PermissionTransferDataObject permissionInfo, Guid currentUserId, bool IsDeleted)
        {
            RESTResult result = new RESTResult { Code = RESTStatus.Success };
            if (Guid.Empty.Equals(permissionInfo.Id) || permissionInfo.Id == null)
            {
                //TODO:Add a new department
                var permissionEntity = Mapper.Map<PermissionTransferDataObject, Permission>(permissionInfo);
                if (permissionInfo.PermissionData != null)
                {
                    permissionInfo.PermissionData.Id = Guid.NewGuid();
                    permissionInfo.PermissionData.ParentId = permissionInfo.PermissionData.ParentId != null ? permissionInfo.PermissionData.ParentId : Guid.Empty;
                    permissionEntity.DataXml = _serializer.Serialize(permissionInfo.PermissionData);
                }
                permissionEntity.CreateTime = DateTime.Now;
                permissionEntity.CreateByUserId = currentUserId;
                result.Data = await _permissionRepository.InsertAsync(permissionEntity);

            }
            else
            {
                //TODO:Update department
                var permissionEntity = await _permissionRepository.GetAsync(permissionInfo.Id.Value);
                permissionEntity.Name = permissionInfo.Name;
                permissionEntity.ModifyByUserId = currentUserId;
                permissionEntity.ModifyTime = DateTime.Now;
                permissionEntity.IsDeleted = IsDeleted;
                permissionEntity.DataXml = permissionInfo.PermissionData != null ? _serializer.Serialize(permissionInfo.PermissionData) : permissionEntity.DataXml;
                result.Data = await _permissionRepository.UpdateAsync(permissionEntity);
            }

            if (result.Data != null)
            {
                return result;
            }
            result.Code = RESTStatus.Failed;
            return result;
        }

        [UnitOfWork(IsDisabled = true)]
        public Task<RESTResult> GetPermissionByPagingAsync(Guid currentUserId, int pageIndex, int pageSize)
        {
            RESTResult result = new RESTResult { Code = RESTStatus.Success };
            var permissionLists = _permissionRepository.GetAllList(item => item.IsDeleted.Equals(false)).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            List<PermissionTransferDataObject> permissionDOTList = new List<PermissionTransferDataObject>();
            foreach (var item in permissionLists)
            {
                var permissionDTO = Mapper.Map<Permission, PermissionTransferDataObject>(item);
                permissionDTO.PermissionData = string.IsNullOrEmpty(item.DataXml) ? null : _serializer.Deserialize<PageMenu>(item.DataXml);
                permissionDOTList.Add(permissionDTO);
            }
            result.Data = permissionDOTList;
            return Task.FromResult(result);
        }

        public async Task<RESTResult> SetPermissionToRoleAsync(Guid currentUserId, RolePermissionTransferObject rolePermissionInfo)
        {
            RESTResult result = new RESTResult { Code = RESTStatus.NotData };

            if (rolePermissionInfo.RoleId == null)
            {
                result.Message = "The role id could not be null";
                return result;
            }
            if (rolePermissionInfo.PermissionIds.Contains(null))
            {
                result.Message = "The permision ids could not be null";
                return result;
            }
            //TOOD:Get the current role permission
            var rolePermissions = await _rolePermissionRepository.GetAllListAsync(item => item.RoleId.Equals(rolePermissionInfo.RoleId));
            //TODO: remove the exist role permission            
            foreach (var item in rolePermissions)
            {
                await _rolePermissionRepository.DeleteAsync(item);
            }
            RolePermission rolePermissionEntity;
            foreach (var permissionId in rolePermissionInfo.PermissionIds)
            {
                //TODO: Add new permission;
                rolePermissionEntity = new RolePermission
                {
                    RoleId = rolePermissionInfo.RoleId.Value,
                    PermissionId = permissionId.Value,
                    CreateByUserId = currentUserId,
                    CreateTime = DateTime.Now
                };
                await _rolePermissionRepository.InsertAsync(rolePermissionEntity);
            }
            result.Code = RESTStatus.Success;
            result.Message = "Set permission to role successful";
            return result;
        }

        public async Task<RESTResult> GetMenusToCurrentUser(Guid userId)
        {
            RESTResult result = new RESTResult { Code = RESTStatus.Success };
            var permissions = await GetCurrentUserPermissions(userId);
            List<PageMenu> pages = new List<PageMenu>();
            foreach (var item in permissions)
            {
                if (!string.IsNullOrEmpty(item.DataXml))
                {
                    pages.Add(_serializer.Deserialize<PageMenu>(item.DataXml));
                }
            }
            List<PageDataTransferObject> pageDTOs = new List<PageDataTransferObject>();
            RecursivePage(pages.OrderBy(c => c.ParentId).ToList(), pageDTOs, true);
            result.Data = pageDTOs;
            return result;
        }

        /// <summary>
        /// This method need to be optimzied, it should be more smarted.
        /// </summary>
        /// <param name="pagemenus"></param>
        /// <param name="childs"></param>
        /// <param name="isRoot"></param>
        private void RecursivePage(List<PageMenu> pagemenus, List<PageDataTransferObject> childs, bool isRoot)
        {
            if (isRoot)
            {
                if (childs.Count <= 0)
                {
                    //TODO:add the root
                    foreach (var item in pagemenus)
                    {
                        if (item.ParentId.Equals(Guid.Empty))
                        {
                            //It's a parent
                            if (childs.Where(c => c.Id.Equals(item.Id)).Count() > 0)
                            {
                                continue;
                            }
                            childs.Add(Mapper.Map<PageMenu, PageDataTransferObject>(item));
                        }
                    }
                    RecursivePage(pagemenus, childs, false);
                }
            }
            else
            {
                foreach (var item in childs)
                {
                    var childItems = pagemenus.Where(c => c.ParentId.Equals(item.Id));
                    if (childItems.Count() > 0)
                    {
                        //TODO:add the child item;
                        foreach (var childItem in childItems)
                        {
                            item.Child.Add(Mapper.Map<PageMenu, PageDataTransferObject>(childItem));
                            RecursivePage(pagemenus, item.Child, false);
                        }
                    }
                }

            }
        }
        #endregion

        #region Help Method
        //[UnitOfWork(IsDisabled = true)]
        private async Task<User> GetUserByAccountName(string accountName)
        {
            var userEntities = await _userRepository.GetAllListAsync(item => item.AccountEmail == accountName || item.AccountCode == accountName || item.AccountPhone == accountName);
            if (userEntities.Count < 0)
            {
                throw new ArgumentException($"The user {accountName} doesn's exist");
            }

            return userEntities.FirstOrDefault();
        }

        [UnitOfWork(IsDisabled = true)]
        private async Task<IEnumerable<Role>> GetRolesWithCurrentUser(Guid userId, Guid departmentId)
        {
            var userRoles = await _userRoleRepository.GetAllListAsync(item => item.UserId.Equals(userId));
            var roleIds = userRoles.Select(item => item.RoleId);
            if (!Guid.Empty.Equals(departmentId))
            {
                //TODO:Get department roles
                var departmentRoles = await _roleRepository.GetAllListAsync(item => item.DepartmentId.Equals(departmentId) && item.IsDeleted.Equals(false) || item.DepartmentId.Equals(Guid.Empty) && item.Level != RoleLevelStatus.SupperAdmin && item.IsDeleted.Equals(false));
                return departmentRoles;

            }
            var roles = await _roleRepository.GetAllListAsync(item => roleIds.Contains(item.Id) && item.IsDeleted.Equals(false));
            return roles;
        }

        private async Task<Department> GetCurrentUserDepartment(Guid userId)
        {
            var userDepartments = await _userDepartmentReposiotry.GetAllListAsync(item => item.UserId.Equals(userId));
            if (userDepartments.Count > 0)
            {
                return await _departmentReposiotry.GetAsync(userDepartments.First().DepartmentId);
            }
            return null;
        }

        private async Task<IEnumerable<Permission>> GetCurrentUserPermissions(Guid userId)
        {
            var roleIds = (await _userRoleRepository.GetAllListAsync(item => item.UserId.Equals(userId))).Select(item => item.RoleId);

            var permissionId = (await _rolePermissionRepository.GetAllListAsync(item => roleIds.Contains(item.RoleId))).Select(item => item.PermissionId);

            var permissions = await _permissionRepository.GetAllListAsync(item => permissionId.Contains(item.Id));

            return permissions;
        }

        private string GeneralPassword(string email)
        {
            return email.Split('@')[0] + "123";
        }

        private async Task SetUserAndDepartmentMapAsync(Guid currentUserId, Guid userId, Guid? departmentId)
        {
            if (departmentId != null && !departmentId.Equals(Guid.Empty))
            {
                //TODO: we are allow one user have a one department, then we will limit the user to add multiple department. 
                var userDepartmentEntities = await _userDepartmentReposiotry.GetAllListAsync(item => item.UserId.Equals(userId) && item.DepartmentId.Equals(departmentId.Value));
                if (userDepartmentEntities.Count > 0)
                {
                    var userDepartmentEntity = userDepartmentEntities.First();
                    userDepartmentEntity.DepartmentId = departmentId.Value;
                    await _userDepartmentReposiotry.UpdateAsync(userDepartmentEntity);
                }
                else
                {
                    UserDepartment userDepartMap = new UserDepartment()
                    {
                        CreateByUserId = currentUserId,
                        CreateTime = DateTime.UtcNow,
                        DepartmentId = departmentId.Value,
                        UserId = userId
                    };
                    await _userDepartmentReposiotry.InsertAsync(userDepartMap);
                }

            }
        }

        private async Task RemoveBeforeUserRole(Guid userId)
        {
            await _userRoleRepository.DeleteAsync(item => item.UserId.Equals(userId));
        }
        private Task SetUserAndRoleMapAsync(Guid currentUserId, Guid userId, List<Guid?> roleIds)
        {
            UserRole userRoleMap;
            if (roleIds == null)
            {
                return Task.CompletedTask;
            }
            foreach (var roleId in roleIds)
            {
                if (roleId == null && !roleId.Equals(Guid.Empty))
                {
                    continue;
                }
                userRoleMap = new UserRole()
                {
                    CreateByUserId = currentUserId,
                    CreateTime = DateTime.UtcNow,
                    RoleId = roleId.Value,
                    UserId = userId,
                    ModifyByUserId = currentUserId,
                    ModifyTime = DateTime.UtcNow
                };
                _userRoleRepository.Insert(userRoleMap);
            }
            return Task.CompletedTask;
        }
        #endregion
    }
}
