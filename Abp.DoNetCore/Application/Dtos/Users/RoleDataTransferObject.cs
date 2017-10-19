using Abp.AutoMapper;
using Abp.DoNetCore.Common;
using Abp.DoNetCore.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Abp.DoNetCore.Application.Dtos.Users
{
    [AutoMap(typeof(Role))]
    [DisplayName("role")]
    public class RoleDataTransferObject
    {
        public Guid? Id { get; set; }
        public string Code { set; get; }
        public string Name { set; get; }
        public string Createor { get; set; }
        public DateTime CreateTime { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifyTime { get; set; }
        [JsonIgnore]
        public RoleLevelStatus Level { get; set; }
        private List<PermissionTransferDataObject> _Permissions;
        public List<PermissionTransferDataObject> Permissions
        {
            get
            {
                if (_Permissions == null)
                {
                    _Permissions = new List<PermissionTransferDataObject>();
                }
                return _Permissions;
            }
            set
            {

                _Permissions = value;
            }
        }
        public bool IsDeleted { set; get; }
    }
}
