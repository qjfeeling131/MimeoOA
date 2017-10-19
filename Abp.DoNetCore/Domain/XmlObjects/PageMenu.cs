using ExtendedXmlSerialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Abp.DoNetCore.Domain.XmlObjects
{
    public class PageMenu
    {
        public Guid? Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Link { get; set; }
        public TreeLevel Level { get; set; }
    }
    public class PageMenuConfig : ExtendedXmlSerializerConfig<PageMenu>
    {
    }


    public enum TreeLevel
    {
        Level_1,
        Level_2,
        Level_3,
        Level_4
    }
}
