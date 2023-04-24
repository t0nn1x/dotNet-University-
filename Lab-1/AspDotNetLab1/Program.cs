using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseFileServer();

app.UseFileServer(new FileServerOptions
{
    EnableDirectoryBrowsing = true,
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"static"))
});

app.MapGet("/home/index", () => "Welcome to Home/Index");
app.MapGet("/home/about", () => "Welcome to Home/About");

app.Use(async (context, next) =>
{
    await next.Invoke();

    if (context.Response.StatusCode == 404)
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync("wwwroot/404.html");
    }
});

app.UseMiddleware<LoggerMiddleware>();
app.UseMiddleware<SecretMiddleware>();

app.Run();

public class LoggerMiddleware
{
    private readonly RequestDelegate _next;

    public LoggerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        File.AppendAllText("access.txt", $"{DateTime.Now} {context.Request.Path}\n");
        await _next(context);
    }
}

public class SecretMiddleware
{
    private readonly RequestDelegate _next;

    public SecretMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsynca(HttpContext context)
    {
        if (context.Request.Path.Value.Contains("/secret-571743235872348"))
        {
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync("Welcome to the secret page!");
        }
        else
        {
            await _next(context);
        }
    }
}
