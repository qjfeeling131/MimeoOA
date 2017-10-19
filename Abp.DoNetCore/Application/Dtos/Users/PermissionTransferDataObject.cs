using Abp.AutoMapper;
using Abp.DoNetCore.Domain;
using Abp.DoNetCore.Domain.XmlObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Abp.DoNetCore.Application.Dtos.Users
{
    [AutoMap(typeof(Permission))]
    [DisplayName("permission")]
    public class PermissionTransferDataObject
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public PageMenu PermissionData { get; set; }
        public int ProrityLevel { get; set; }
    }
}
