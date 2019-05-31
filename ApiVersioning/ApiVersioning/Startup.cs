using ApiVersioning.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace ApiVersioning
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddApiVersioning(o =>
            {
                o.UseApiBehavior = false;
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);

                o.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("x-api-version"),
                    new QueryStringApiVersionReader(),
                    new UrlSegmentApiVersionReader());
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen();
            services.ConfigureOptions<ConfigureSwaggerOptions>();
        }

        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });

            app.UseMvc();
        }
    }
}
