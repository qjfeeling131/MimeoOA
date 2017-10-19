using Abp.Domain.Entities;
using Abp.DoNetCore.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.DoNetCore.Domain
{
   public class Permission:Entity
    {
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        public Guid CreateByUserId { get; set; }
        public DateTime? ModifyTime { get; set; }
        public Guid ModifyByUserId { get; set; }
        public PermissionStatus Status { get; set; }
        public string DataXml { get; set; }
        public string ProrityLevel { get; set; }
        public bool IsDeleted { get; set; }
    }
}


