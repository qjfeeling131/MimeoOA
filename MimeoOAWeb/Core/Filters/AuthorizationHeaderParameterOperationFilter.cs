using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;

namespace MimeoOAWeb.Core.Filters
{
    public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {

            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            //var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAuthorizationFilter);
            var isAuthorized = context.ApiDescription.ActionAttributes().Where(item => item is AuthorizeAttribute).Any();
            var isAuthorizedController = context.ApiDescription.ControllerAttributes().Where(item => item is AuthorizeAttribute).Any();
            var allowAnonymous = context.ApiDescription.ActionAttributes().Where(item => item is AllowAnonymousAttribute).Any();

            if ((isAuthorized && !allowAnonymous) || (isAuthorizedController && !allowAnonymous))
            {
                if (operation.Parameters == null)
                {
                    operation.Parameters = new List<IParameter>();
                }
                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "Authorization",
                    In = "header",
                    Description = @"access token; eg:Bearer[space][toekn]",
                    Required = true,
                    Type = "string"
                });
            }
        }
    }
}
