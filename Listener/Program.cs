using Listener.Consumer;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);
var appName = typeof(Program).Assembly.GetName().Name;

// Add services to the container.

builder.Services.AddControllers();

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
    x.AddConsumer<MessageConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
        cfg.UseMessageRetry(retryConf =>
        {
            retryConf.Interval(5, TimeSpan.FromMinutes(200));
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.Run();