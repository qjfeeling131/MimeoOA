using Abp.AutoMapper;
using Abp.DoNetCore.Common;
using Abp.DoNetCore.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Abp.DoNetCore.Application.Dtos.Users
{
    [AutoMap(typeof(UserInfo))]
    [DisplayName("userInfo")]
    public class UserInfoDataTransferObject
    {
        //[Required]
        //public string UserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string NickName { get; set; }

        public CardStatus CardType { get; set; }

        public string CardNo { get; set; }

        public string Phone { get; set; }

        public string Tel { get; set; }

        public string Address { get; set; }

        public DateTime BirthDate { get; set; }

        public string ExtendInfo { get; set; }
    }
}
