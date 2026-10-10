
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

TypeAdapterConfig.GlobalSettings.Scan(typeof(PollMappingConfig).Assembly);

builder.Services.AddOpenApi();

builder.Services.AddServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "SurveyBasket API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
