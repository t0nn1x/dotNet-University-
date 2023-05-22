using AspDotNetLab3;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("conf.json");

builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), 
    builder.Configuration.GetRequiredSection("LogFile").Value ?? "log.txt"));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

app.UseSession();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.Use(async (context, next) =>
{
    var remoteIpAddress = Utils.GetClientIpAddress(context);
    app.Logger.LogInformation($"���� ������: {context.Request.Path} | ���� �� ���: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")} | IP-������ �����������: {remoteIpAddress}");
    await next.Invoke(context);
});

app.MapGet("/", async (HttpContext context, IConfiguration configuration) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(Utils.GetWrappedByTag("h1", configuration["Title"])
        + Utils.GetWrappedByTag("h3", "������� �������"));
});

app.Map("/{controller}/{action}/{id?}",
    async (string controller, string action, int? id, IConfiguration configuration, HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    var additionalInformation = $"����� ����������: {controller}, ����� 䳿: {action}, ";
    additionalInformation += id is null
        ? "Id �� ��� ���������" 
        : $"�������� Id: {id}";
    var responseHtml = Utils.GetWrappedByTag("h1", configuration["Title"]) 
        + Utils.GetWrappedByTag("p", additionalInformation);
    await context.Response.WriteAsync(responseHtml);
});

app.Map("/{language}/{controller}/{action}/{id?}", 
    async (string language, string controller, string action, int? id, IConfiguration configuration, HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    var additionalInformation = $"�������� language: {language}, ����� ����������: {controller}, ����� 䳿: {action}, ";
    additionalInformation += id is null 
        ? "Id �� ��� ���������"
        : $"�������� Id: {id}";
    var responseHtml = Utils.GetWrappedByTag("h1", configuration["Title"])
        + Utils.GetWrappedByTag("p", additionalInformation);
    await context.Response.WriteAsync(responseHtml);
});

app.Map("/Session/Add/{parameter}/{value}", 
    async (string parameter, string value, IConfiguration configuration, HttpContext context) =>
{
    context.Session.SetString(parameter, value);
    context.Response.ContentType = "text/html; charset=utf-8";
    var additionalInformation = $"��������: {parameter} � ��������� {value} ���� ������ ������ �� ���";
    var responseHtml = Utils.GetWrappedByTag("h1", configuration["Title"])
        + Utils.GetWrappedByTag("p", additionalInformation);
    await context.Response.WriteAsync(responseHtml);
});

app.Map("/Session/View/{parameter}", 
    async (string parameter, IConfiguration configuration, HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    var value = context.Session.GetString(parameter);
    var additionalInformation = value is null
        ? $"�� �������� �������� �� ������ {parameter} � ���"
        : $"�������� ��������� {parameter}, ������� � ���: {value}";
    var responseHtml = Utils.GetWrappedByTag("h1", configuration["Title"])
        + Utils.GetWrappedByTag("p", additionalInformation);
    await context.Response.WriteAsync(responseHtml);
});

app.Map("/Cookie/Add/{parameter}/{value}", 
    async (string parameter, string value, IConfiguration configuration, HttpContext context) =>
{
    context.Response.Cookies.Append(parameter, value);
    context.Response.ContentType = "text/html; charset=utf-8";
    var additionalInformation = $"��������: {parameter} � ��������� {value} ���� ������ ������ � ���";
    var responseHtml = Utils.GetWrappedByTag("h1", configuration["Title"])
        + Utils.GetWrappedByTag("p", additionalInformation);
    await context.Response.WriteAsync(responseHtml);
});

app.Map("/Cookie/View/{parameter}",
    async (string parameter, IConfiguration configuration, HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    context.Request.Cookies.TryGetValue(parameter, out var value);
    var additionalInformation = value is null
        ? $"�� �������� �������� �� ������ {parameter} � ���"
        : $"�������� ��������� {parameter}, ������� � ���: {value}";
    var responseHtml = Utils.GetWrappedByTag("h1", configuration["Title"])
        + Utils.GetWrappedByTag("p", additionalInformation);
    await context.Response.WriteAsync(responseHtml);
});

app.Run();
