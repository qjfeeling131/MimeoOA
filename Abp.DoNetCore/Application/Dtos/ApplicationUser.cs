using Abp.AutoMapper;
using Abp.DoNetCore.Common;
using Abp.DoNetCore.Domain;
using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.DoNetCore.Application.Dtos
{
    [AutoMap(typeof(User))]
    public class ApplicationUser
    {
        public string AccountName { get; set; }
        public string Password { get; set; }
    }
}
