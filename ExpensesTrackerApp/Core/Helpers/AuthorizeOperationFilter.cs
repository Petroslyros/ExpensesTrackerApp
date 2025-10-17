using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ExpensesTrackerApp.Core.Helpers
{
    public class AuthorizeOperationFilter : IOperationFilter
    {
        //customizes Swagger so that endpoints protected by [Authorize] are clearly shown as secured,
        //display correct responses, and integrate seamlessly with JWT Bearer tokens.
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authAttributes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Distinct();

            if (authAttributes.Any())
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                // add security requirement 
                operation.Security = new List<OpenApiSecurityRequirement>();

                var roles = context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>()
                    .SelectMany(attr => attr.Roles!.Split(','));

                //add the security requirment for JWT Bearer with specified roles
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Description = "Add token to header",
                            Name = "Authorization",
                            Type = SecuritySchemeType.Http,
                            In = ParameterLocation.Header,
                            Scheme = JwtBearerDefaults.AuthenticationScheme,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        roles.ToList()

                    }
                });
            }
        }
    }
}
