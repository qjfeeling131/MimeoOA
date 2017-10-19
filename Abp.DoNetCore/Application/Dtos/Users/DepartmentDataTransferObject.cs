using Abp.AutoMapper;
using Abp.DoNetCore.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Globalization;
using Newtonsoft.Json;

namespace Abp.DoNetCore.Application.Dtos.Users
{
    [AutoMap(typeof(Department))]
    public class DepartmentDataTransferObject
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string Createor { get; set; }

        public DateTime CreateTime { get; set; }

        public string Modifier { get; set; }

        public DateTime ModifyTime { get; set; }

    }
}