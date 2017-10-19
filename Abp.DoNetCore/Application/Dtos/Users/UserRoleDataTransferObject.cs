using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.DoNetCore.Application.Dtos.Users
{
    public class UserRoleDataTransferObject
    {
        public Guid? Id { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
