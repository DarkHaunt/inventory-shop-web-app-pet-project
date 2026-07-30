using Scalar.AspNetCore;

namespace InventoryShop.Web.Bindings;

public static class ApplicationExtensions
{
   public static IEndpointRouteBuilder MapDebugUI(this IEndpointRouteBuilder app)
   {
      app.MapOpenApi();
      app.MapScalarApiReference(options =>
      {
         options.Title = "InventoryShop API";
         options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.Http, ScalarClient.HttpClient);
         options.Authentication = new ScalarAuthenticationOptions
         {
            PreferredSecuritySchemes = new List<string> {"Bearer"}
         };
      });
      
      return app;
   }  
}