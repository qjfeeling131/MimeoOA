using Abp.AutoMapper;
using Abp.DoNetCore.Common;
using Abp.DoNetCore.Domain;
using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Abp.DoNetCore.Application.Dtos.Users
{
    [AutoMapFrom(typeof(User))]
    [DisplayName("user")]
    public class UserDataTransferObject
    {
        public Guid? Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public string Phone { get; set; }
        public UserActiveStatus Actived { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }
        public UserInfoDataTransferObject UserInfo { get; set; }
        private List<RoleDataTransferObject> _roles;
        public List<RoleDataTransferObject> Roles
        {
            get
            {
                if (_roles == null)
                {
                    _roles = new List<RoleDataTransferObject>();
                }
                return _roles;
            }
            set
            {
                _roles = value;
            }
        }
        public DepartmentDataTransferObject Department { get; set; }
        private List<PermissionTransferDataObject> _permissions;
        public List<PermissionTransferDataObject> Permissions
        {
            get
            {
                if (_permissions == null)
                {
                    _permissions = new List<PermissionTransferDataObject>();
                }
                return _permissions;
            }
            set
            {
                _permissions = value;
            }
        }

    }
}
