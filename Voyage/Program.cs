using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Voyage.Business;
using Voyage.Data;
using Voyage.Data.TableModels;
using Voyage.Services;
using Voyage.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

//for injecting sessions into views
builder.Services.AddHttpContextAccessor();

//all pages secured automatically unless using AllowAnonymous attribute on controller
builder.Services.AddAuthorization();
//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = options.DefaultPolicy;
//});

// logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

//sessions
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true; // prevents JS access
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

//CORS - used for Electron container
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowElectron", policy =>
//    {
//        policy.WithOrigins("app://-") // electron scheme
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//    });
//});

//CORS - used for views to communicate with controllers
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

//database config
builder.Services.AddDbContext<_AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
                                .ConfigureWarnings(warnings => 
                                    warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

//identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<_AppDbContext>()
                .AddDefaultTokenProviders();  // This is required for password reset tokens

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/User/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.SlidingExpiration = true;
});

//utilities


//services
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<RoleSeeder>();
builder.Services.AddScoped<SectionSeeder>();

//business layer
builder.Services.AddScoped<TicketsBLL>();
builder.Services.AddScoped<HrBLL>();
builder.Services.AddScoped<AccountBLL>();
builder.Services.AddScoped<PermissionsBLL>();

//data layer
builder.Services.AddScoped<TicketsDAL>();
builder.Services.AddScoped<HrDAL>();
builder.Services.AddScoped<AccountDAL>();
builder.Services.AddScoped<PermissionsDAL>();

//Config Constants
Constants.Initialize(builder.Configuration);





var app = builder.Build();



//seed default values on startup
using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
    await roleSeeder.CreateGlobalRoles();

    var sectionSeeder = scope.ServiceProvider.GetRequiredService<SectionSeeder>();
    await sectionSeeder.CreateGlobalSections();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseHsts();

//app.UseCookiePolicy(new CookiePolicyOptions
//{
//    Secure = CookieSecurePolicy.Always, // only over HTTPS
//    HttpOnly = HttpOnlyPolicy.Always,
//    MinimumSameSitePolicy = SameSiteMode.Strict
//});

app.UseStaticFiles();

app.UseRouting();



//app.UseCors("AllowElectron");

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CompanySessionMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Website}/{action=Index}/{id?}");

app.Run();
