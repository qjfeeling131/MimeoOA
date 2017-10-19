using Abp.Collections.Extensions;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.AutoMapper
{
    public class AutoMapFromAttribute : AutoMapAttributeBase
    {
        public MemberList MemberList { get; set; } = MemberList.Destination;
        private Dictionary<string, Action<IMemberConfigurationExpression>> selfMemberOptions;
        public AutoMapFromAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {

        }

        public AutoMapFromAttribute(MemberList memberList, params Type[] targetTypes)
            : this(targetTypes)
        {
            MemberList = memberList;
        }
        public AutoMapFromAttribute(Dictionary<string, Action<IMemberConfigurationExpression>> selfMemberOptions, params Type[] targetTypes) : base(targetTypes)
        {
            this.selfMemberOptions = selfMemberOptions;
        }
        public override void CreateMap(IMapperConfigurationExpression configuration, Type type)
        {
            if (TargetTypes.IsNullOrEmpty())
            {
                return;
            }

            foreach (var targetType in TargetTypes)
            {
                var configMap = configuration.CreateMap(targetType, type, MemberList.Destination);
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
