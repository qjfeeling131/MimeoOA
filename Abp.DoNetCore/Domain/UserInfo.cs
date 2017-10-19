using Abp.Domain.Entities;
using Abp.DoNetCore.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.DoNetCore.Domain
{
    public class UserInfo : Entity
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public CardStatus CardType { get; set; }
        public string CardNo { get; set; }
        public string Phone { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public string ExtendInfo { get; set; }
    }
}
