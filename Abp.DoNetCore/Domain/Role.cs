using Abp.Domain.Entities;
using Abp.DoNetCore.Common;
using System;

namespace Abp.DoNetCore.Domain
{

    public class Role : Entity
    {
        public Guid DepartmentId { set; get; }
        public DateTime? CreateTime { set; get; }
        public Guid CreateByUserId { set; get; }
        public DateTime? ModifyTime { set; get; }
        public Guid ModifyByUserId { set; get; }
        public string Code { set; get; }
        public string Name { set; get; }
        public RoleLevelStatus Level { get; set; }
        public bool IsDeleted { set; get; }
    }
}
