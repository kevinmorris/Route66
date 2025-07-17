using System.Xml.Linq;
using Api.GraphQL;
using Api.State;
using Services;
using Services.Models;
using Services.Translators;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddTransient(s => new TN3270Service<IEnumerable<FieldData>>(new Poco3270Translator()));
builder.Services.AddSingleton<TerminalStatePool>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddGraphQLServer()
    .AddDocumentFromFile("GraphQL/schema.graphql")
    .BindRuntimeType<Query>("Query")
    .BindRuntimeType<Mutation>("Mutation")
    .BindRuntimeType<Subscription>()
    .AddInMemorySubscriptions();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(b =>
{
    //TODO: Set your CORS Policy here
    b.SetIsOriginAllowed(_ => true);
    b.AllowAnyHeader();
    b.AllowCredentials();
});

var webSocketOptions = new WebSocketOptions();

//TODO: Set allowed websocket origins here
webSocketOptions.AllowedOrigins.Add("http://localhost:3000");
webSocketOptions.AllowedOrigins.Add("http://localhost:7149");
webSocketOptions.AllowedOrigins.Add("http://localhost:63343");
webSocketOptions.AllowedOrigins.Add("http://localhost:5173");

app.UseWebSockets(webSocketOptions);

app.UseSession();
app.MapControllers();
app.MapGraphQL();

app.Run();
