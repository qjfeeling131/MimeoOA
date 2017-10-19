using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.DoNetCore.Domain
{
    public class UserRole : Entity
    {
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
        public DateTime? CreateTime { get; set; }
        public Guid CreateByUserId { get; set; }
        public DateTime? ModifyTime { get; set; }
        public Guid ModifyByUserId { get; set; }
    }
}
