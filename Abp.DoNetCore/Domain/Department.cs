using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Abp.DoNetCore.Domain
{
    public class Department : Entity
    {
        public DateTime? CreateTime { set; get; }
        public Guid CreateByUserId { set; get; }
        public DateTime? ModifyTime { set; get; }
        public Guid ModifyByUserId { set; get; }
        [Required]
        public string Name { set; get; }
        public bool IsDeleted { set; get; }
    }
}
