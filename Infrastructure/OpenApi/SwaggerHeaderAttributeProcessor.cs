using Newtonsoft.Json.Schema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Reflection;
using NJsonSchema;
using JsonSchema = NJsonSchema.JsonSchema;

namespace Infrastructure.OpenApi
{
    public class SwaggerHeaderAttributeProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            if(context.MethodInfo.GetCustomAttributes(typeof(SwaggerHearderAttribute)) is SwaggerHearderAttribute swaggerHearder)
            {
                var parameters = context.OperationDescription.Operation.Parameters;

                var existstingParam = parameters
                    .FirstOrDefault(p => p.Kind == NSwag.OpenApiParameterKind.Header && p.Name == swaggerHearder.HeaderName);

                if (existstingParam is not null)
                {
                    parameters.Remove(existstingParam);
                }

                parameters.Add(new OpenApiParameter
                {
                    Name = swaggerHearder.HeaderName,
                    Kind = OpenApiParameterKind.Header,
                    Description = swaggerHearder.Description,
                    IsRequired = true,
                    Schema = new JsonSchema
                    {
                        Type = JsonObjectType.String, 
                        Default = swaggerHearder.DefaultValue,
                    }
                });
            }
            return true;
        }
    }
}
