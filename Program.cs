using SpoRE.Middleware;
using SpoRE.Models.Settings;
using SpoRE.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddScoped<IAccountService, AccountService>(); // nodig zodat dit zo maar meegegeven kan worden in constructors, die interface is misschien overbodig
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();// nodig zodat dit zo maar meegegeven kan worden in constructors

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<JwtMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
