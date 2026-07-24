using System.Text;
using InventoryShop.Application.Common;
using InventoryShop.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using WebApi.OpenApi;

namespace InventoryShop.Web.Bindings;

public static class OtherBindings
{
   public static IServiceCollection AddAndSetupOpenApi(this IServiceCollection services)
   {
      services.AddOpenApi(options =>
      {
         options.AddDocumentTransformer((document, context, cancellationToken) =>
         {
            document.Info.Version = "1.0";
            document.Info.Title = "Inventory Shop API";
            document.Info.Description = "Web ASP.NET application implementation of inventory / shop system in some game";
            document.Info.Contact = new OpenApiContact
            {
               Name = "Yaroslav Kyzyk",
               
            };
            document.Info.License = new OpenApiLicense
            {
               Name = "MIT License",
               Url = new Uri("https://opensource.org/licenses/MIT")
            };
            return Task.CompletedTask;
         });
         options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
         options.AddOperationTransformer<AuthorizeOperationTransformer>();
      });
      
      return services;
   }
   
   public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
   {
      JwtOptions jwtOptions = configuration
                                 .GetSection(JwtOptions.SectionName)
                                 .Get<JwtOptions>()
                              ?? throw new InvalidOperationException("Jwt configuration section is missing.");
      
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
         .AddJwtBearer(options =>
         {
            options.TokenValidationParameters = new TokenValidationParameters
            {
               ValidateAudience = false,
               ValidateIssuer = false,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(jwtOptions.SecretKey)) 
            };
         });

      return services;
   }
   
   public static IServiceCollection AddAuthorization(this IServiceCollection services, IConfiguration configuration)
   {
      services.AddAuthorization(options =>
      {
         options.AddPolicy(Policies.RequireUser, policy =>
            policy
               .RequireAuthenticatedUser()
               .RequireRole(Roles.User, Roles.Admin));

         options.AddPolicy(Policies.RequireAdmin, policy =>
            policy
               .RequireAuthenticatedUser()
               .RequireRole(Roles.Admin));
      });
      
      return services;
   }
}