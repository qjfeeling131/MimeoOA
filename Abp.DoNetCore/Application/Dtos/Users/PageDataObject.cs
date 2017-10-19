using Abp.DoNetCore.Domain.XmlObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.DoNetCore.Application.Dtos.Users
{
    [AutoMapper.AutoMap(typeof(PageMenu))]
    public class PageDataObject
    {
        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public string Link { get; set; }

        private List<PageDataObject> _child;
        public List<PageDataObject> Child
        {
            get
            {
                if (_child == null)
                {
                    _child = new List<PageDataObject>();
                }
                return _child;
            }
            set
            {
                _child = value;
            }
        }
    }
}
