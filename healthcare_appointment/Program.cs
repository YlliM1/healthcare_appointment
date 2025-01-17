using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using healthcare_appointment.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Configure the default login and access denied paths
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";  // Ensure the login path is correct
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";

    // This will ensure that users are redirected back to the page they were originally trying to access
    options.Events.OnRedirectToLogin = async context =>
    {
        // If a user is not authorized and they are not already in the Identity area
        if (!context.Request.Path.StartsWithSegments("/Identity"))
        {
            // If a ReturnUrl is provided, redirect there, else go to the login page
            var returnUrl = context.Request.Query["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
            {
                context.Response.Redirect(returnUrl);
            }
            else
            {
                context.Response.Redirect("/Identity/Account/Login");
            }
        }
    };
});

// Add runtime compilation for Razor Pages
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Default route for controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();



app.Run();
