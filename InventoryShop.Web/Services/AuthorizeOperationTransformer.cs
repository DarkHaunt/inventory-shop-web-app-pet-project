using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace WebApi.OpenApi;

public sealed class AuthorizeOperationTransformer : IOpenApiOperationTransformer
{
   public Task TransformAsync(
      OpenApiOperation operation,
      OpenApiOperationTransformerContext context,
      CancellationToken cancellationToken)
   {
      var metadata = context.Description.ActionDescriptor.EndpointMetadata;

      var hasAuthorize    = metadata.OfType<IAuthorizeData>().Any();
      var hasAllowAnonymous = metadata.OfType<IAllowAnonymous>().Any();

      if (hasAuthorize && !hasAllowAnonymous)
      {
         operation.Security?.Add(new OpenApiSecurityRequirement
         {
            [new OpenApiSecuritySchemeReference("Bearer")] = []
         });
      }

      return Task.CompletedTask;
   }
}