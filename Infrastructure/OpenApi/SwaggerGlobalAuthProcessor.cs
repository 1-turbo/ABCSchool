using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Namotion.Reflection;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Reflection;

namespace Infrastructure.OpenApi
{
    public class SwaggerGlobalAuthProcessor(string schema) : IOperationProcessor
    {
        private readonly string _schema = schema;
        public SwaggerGlobalAuthProcessor()
            : this(JwtBearerDefaults.AuthenticationScheme)
        {

        }
        public bool Process(OperationProcessorContext context)
        {
            // Null check for context and its properties
            if (context == null ||
                context.OperationDescription?.Operation == null ||
                !(context is AspNetCoreOperationProcessorContext aspNetContext))
            {
                return true;
            }

            var list = aspNetContext
                .ApiDescription?.ActionDescriptor?
                .TryGetPropertyValue<IList<object>>("EndpointMetadata");

            if (list is not null)
            {
                if (list.OfType<AllowAnonymousAttribute>().Any())
                {
                    return true;
                }

                // Safely handle security requirements
                if (context.OperationDescription.Operation.Security == null ||
                    context.OperationDescription.Operation.Security.Count == 0)
                {
                    context.OperationDescription.Operation.Security ??= new List<OpenApiSecurityRequirement>();
                    context.OperationDescription.Operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        {
                            _schema,
                            Array.Empty<string>()
                        }
                    });
                }
            }
            return true;
        }
    }

    public static class ObjectExtensions
    {
        public static T TryGetPropertyValue<T>(this object obj, string propertyName, T defaultValue = default) =>
            obj?.GetType().GetRuntimeProperty(propertyName) is PropertyInfo propertyInfo
                ? (T)propertyInfo.GetValue(obj)
                : defaultValue;
    }
}