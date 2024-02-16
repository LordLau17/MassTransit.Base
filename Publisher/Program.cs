using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var appName = typeof(Program).Assembly.GetName().Name;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument(x =>
{
    x.Title = appName;
    x.Version = "1.0.0";
});

builder.Host.UseSerilog((context, conf) =>
{
    conf
        .WriteTo.Seq(context.Configuration["SeqServerUrl"], Serilog.Events.LogEventLevel.Debug)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentUserName();
});

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();