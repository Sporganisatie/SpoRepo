using SpoRE.Infrastructure.Database;
using SpoRE.Middleware;
using SpoRE.Models.Settings;
using SpoRE.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings")); //TODO remove jwt secret
builder.Services.AddScoped<AccountService, AccountService>(); // nodig zodat dit zo maar meegegeven kan worden in constructors, die interface is misschien overbodig
builder.Services.AddScoped<AccountClient, AccountClient>(); // TODO kijken of dit hier weg kan en new AccountClient in AccountService doen of zo
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();// nodig zodat dit zo maar meegegeven kan worden in constructors

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
