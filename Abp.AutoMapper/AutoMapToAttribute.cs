using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Abp.Collections.Extensions;

namespace Abp.AutoMapper
{
    public class AutoMapToAttribute : AutoMapAttributeBase
    {
        public MemberList MemberList { get; set; } = MemberList.Source;
        private Dictionary<string, Action<IMemberConfigurationExpression>> selfMemberOptions;
        public AutoMapToAttribute(params Type[] targetTypes) : base(targetTypes)
        {

        }
        public AutoMapToAttribute(Dictionary<string, Action<IMemberConfigurationExpression>> selfMemberOptions, params Type[] targetTypes) : base(targetTypes)
        {
            this.selfMemberOptions = selfMemberOptions;
        }
        public AutoMapToAttribute(MemberList memberList, params Type[] targetTypes)
           : this(targetTypes)
        {
            MemberList = memberList;
        }
        public override void CreateMap(IMapperConfigurationExpression configuration, Type type)
        {
            if (TargetTypes.IsNullOrEmpty())
            {
                return;
            }

            foreach (var targetType in TargetTypes)
            {
                var configMap = configuration.CreateMap(type, targetType, MemberList);
                if (selfMemberOptions != null)
                {
                    foreach (var selfMemberOption in selfMemberOptions)
                    {
                        configMap.ForMember(selfMemberOption.Key, selfMemberOption.Value);
                    }

                }
            }
        }
    }
}
