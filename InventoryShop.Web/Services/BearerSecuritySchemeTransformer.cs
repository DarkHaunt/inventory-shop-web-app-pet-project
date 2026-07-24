using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace WebApi.OpenApi;

public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
   public Task TransformAsync(
      OpenApiDocument document,
      OpenApiDocumentTransformerContext context,
      CancellationToken cancellationToken)
   {
      document.Components ??= new OpenApiComponents();
      document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
      {
         ["Bearer"] = new OpenApiSecurityScheme
         {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header
         }
      };

      return Task.CompletedTask;
   }
}