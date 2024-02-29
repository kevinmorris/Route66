using System.Xml.Linq;
using Api.Models;
using Services;
using Services.Models;
using Services.Translators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton(new TerminalState(
    new TN3270Service<IEnumerable<FieldData>>(new Poco3270Translator()),
    "127.0.0.1",
    3270));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();
app.MapControllers();

app.Run();
