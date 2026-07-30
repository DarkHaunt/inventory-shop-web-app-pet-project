using FluentValidation;
using InventoryShop.Web.Bindings;
using InventoryShop.Web.Services;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAndSetupOpenApi();

builder.Services.AddDomainServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddAuthorization(builder.Configuration);

builder.Services.AddCaching(builder.Configuration);
builder.Services.AddLogging();
builder.Services.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Transient);
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
   app.MapDebugUI();
}
else
{
   app.UseHsts();
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();