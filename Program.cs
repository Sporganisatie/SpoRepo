using SpoRE.Helper;
using SpoRE.Infrastructure.Database;
using SpoRE.Infrastructure.Scrape;
using SpoRE.Middleware;
using SpoRE.Models.Settings;
using SpoRE.Setup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddScoped<DatabaseContext, DatabaseContext>();
builder.Services.AddScoped<Userdata, Userdata>();
builder.Services.AddScoped<Scrape, Scrape>();
builder.Services.AddServicesAndClients();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerLogin();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseSwagger();
    app.UseSwaggerUI(config =>
        {
            config.ConfigObject.AdditionalItems["syntaxHighlight"] = new Dictionary<string, object>
            {
                ["activated"] = false
            };
        });
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
#pragma warning disable 4014 // Disable "Because this call is not awaited..." warning
Task.Run(() => new Scheduler(app.Services).RunTimer());
#pragma warning restore 4014 // Restore the warning

app.Run();
