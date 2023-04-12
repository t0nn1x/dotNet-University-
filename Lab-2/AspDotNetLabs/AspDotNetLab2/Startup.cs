using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<TimerService>();
        services.AddScoped<RandomService>();
        services.AddSingleton<GeneralCounterService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/services/list", async context =>
            {
                var serviceDescriptors = app.ApplicationServices.GetRequiredService<IEnumerable<ServiceDescriptor>>();
                var result = string.Join("\n", serviceDescriptors.Select(x => $"{x.ServiceType.Name}: {x.ImplementationType?.Name ?? x.ImplementationInstance?.GetType().Name ?? "N/A"} ({x.Lifetime})"));

                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(result);
            });

            endpoints.MapGet("/services/timer", async context =>
            {
                var timerService = context.RequestServices.GetRequiredService<TimerService>();
                await context.Response.WriteAsync(timerService.GetCurrentDateTime().ToString());
            });

            endpoints.MapGet("/services/random", async context =>
            {
                var randomService = context.RequestServices.GetRequiredService<RandomService>();
                await context.Response.WriteAsync($"Random Number 1: {randomService.RandomNumber}\nRandom Number 2: {randomService.RandomNumber}");
            });

            endpoints.MapGet("/services/general-counter", async context =>
            {
                var generalCounterService = context.RequestServices.GetRequiredService<GeneralCounterService>();
                generalCounterService.IncrementCounter(context.Request.Path);

                var allCounters = generalCounterService.GetAllCounters();
                var result = string.Join("\n", allCounters.Select(x => $"{x.Key}: {x.Value}"));

                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(result);
            });
        });
    }
}
