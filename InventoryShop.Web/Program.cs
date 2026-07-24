using FluentValidation;
using InventoryShop.Web.Bindings;
using InventoryShop.Web.Services;
using Scalar.AspNetCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDomainServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddAuthorization(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddAndSetupOpenApi();
builder.Services.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Transient);
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddLogging();
builder.Services.AddProblemDetails();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
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
}
else
{
   app.UseHsts();
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();