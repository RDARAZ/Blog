using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Blog.API.Configuration;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerWithJwtAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Blog API",
                Version = "v1",
                Description = "Personal Blog Application API with JWT Authentication",
                Contact = new OpenApiContact
                {
                    Name = "Robert",
                    Email = "robert@blog.com"
                }
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' followed by a space and your JWT token.\n\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if(File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });
        return services;
    }
}
