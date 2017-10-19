using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.DoNetCore.Common;
using AutoMapper.Configuration.Conventions;

namespace Abp.DoNetCore.Domain
{
    public class User : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override Guid Id { get => base.Id; set => base.Id = value; }
        [MapTo("Email")]
        public string AccountEmail { get; set; }
        [MapTo("Code")]
        public string AccountCode { get; set; }
        [MapTo("Phone")]
        public string AccountPhone { get; set; }
        public string Password { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModifyTime { get; set; }
        public Guid ModifyByUserId { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public string LastLoginIP { get; set; }
        [MapTo("Actived")]
        public UserActiveStatus Status { get; set; }
        public bool IsDeleted { get; set; }
    }
}
