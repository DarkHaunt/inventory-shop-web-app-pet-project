using inventory_shop_web_app_pet_project;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
   app.MapOpenApi();
   app.UseSwagger();
   app.UseSwaggerUI();
}
else
{
   app.UseHsts();
}

app.UseHttpsRedirection();

var summaries = new[]
{
   "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
   {
      var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
               DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
               Random.Shared.Next(-20, 55),
               summaries[Random.Shared.Next(summaries.Length)]
            ))
         .ToArray();
      return forecast;
   })
   .WithName("GetWeatherForecast");

app.Run();

namespace inventory_shop_web_app_pet_project
{
   record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
   {
      public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
   }
}