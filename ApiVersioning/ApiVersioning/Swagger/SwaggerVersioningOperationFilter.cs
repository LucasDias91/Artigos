using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace ApiVersioning.Swagger
{
    public class SwaggerVersioningOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            operation.Deprecated = apiDescription.IsDeprecated();

            if (operation.Parameters == null)
            {
                return;
            }

            // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
            // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
            foreach (var parameter in operation.Parameters.OfType<NonBodyParameter>())
            {
                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

                if (parameter.Description == null)
                {
                    parameter.Description = description.ModelMetadata?.Description;
                }

                if (parameter.Default == null)
                {
                    parameter.Default = description.DefaultValue;
                }

                parameter.Required |= description.IsRequired;
            }

            var headerParameter = operation.Parameters.FirstOrDefault(a => a.Name.ToLowerInvariant() == "x-api-version");
            if (headerParameter == null)
            {
                headerParameter = new NonBodyParameter
                {
                    Name = "api-version",
                    In = "header",
                    Type = "string",
                    Required = false
                };

                operation.Parameters.Add(headerParameter);
            }

            headerParameter.Required = false;
        }
    }
}
