using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.DoNetCore.Application.Dtos
{
    public class DigitalAssetItemDataObject
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public int ItemStatus { get; set; }
        public string Description { get; set; }
    }
}
