using Abp.AutoMapper;
using Abp.DoNetCore.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Abp.DoNetCore.Application.Dtos.Users
{
    [AutoMap(typeof(UserDepartment))]
    public class UserDepartmentTransferDataObject
    {
        public Guid DepartmentId { get; set; }

        public Guid UserId { get; set; }
    }
}
