using ConfigurationCatTask;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// add the worker service for test 
builder.Services.AddScoped<Worker>();

builder.Services.AddOptions<ConfigurationCat>()
    .Bind(builder.Configuration.GetSection("ConfigurationCat"))
    .ValidateDataAnnotations();

builder.Services.AddHttpClient<Worker>();

var app = builder.Build();
using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;
var worker = serviceProvider.GetRequiredService<Worker>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
