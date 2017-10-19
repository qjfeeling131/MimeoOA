using Abp.DoNetCore.Domain;
using AutoMapper.Configuration.Conventions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Abp.DoNetCore.Application.Dtos.Users
{
    [AutoMapper.AutoMap(typeof(User))]
    public class RegisterUserDataObject
    {
        [JsonProperty("Email")]
        public string AccountEmail { get; set; }
        [JsonProperty("Code")]
        public string AccountCode { get; set; }
        [JsonProperty("Phone")]
        public string AccountPhone { get; set; }
        [Required]
        public string Password { get; set; }
        public List<Guid?> RoleIds { get; set; }
        public Guid? DepartmentId { get; set; }
        public UserInfoDataTransferObject UserInfo { get; set; }
    }
}
