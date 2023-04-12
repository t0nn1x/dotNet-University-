using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Map("/", () => "Hello World!");
app.UseFileServer();
app.UseFileServer(
    new FileServerOptions
    {
        EnableDirectoryBrowsing = true,
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), @"static")
        )
    }
);

app.UseExceptionHandler(
    app =>
        app.Run(async context =>
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Error 500");
        })
);

public class NotFoundMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _filePath;

    public NotFoundMiddleware(RequestDelegate next, string filePath)
    {
        _next = next;
        _filePath = filePath;
    }

    public async Task Invoke(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == 404)
        {
            context.Request.Path = _filePath;
            await _next(context);
        }
    }
}

app.UseMiddleware<NotFoundMiddleware>("wwwroot/404.html");



app.Run();
