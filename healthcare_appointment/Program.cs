using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using healthcare_appointment.Data;
using healthcare_appointment.Models;

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

    options.Events.OnRedirectToLogin = async context =>
    {
        // If a user is not authorized and they are not already in the Identity area
        if (!context.Request.Path.StartsWithSegments("/Identity"))
        {
            // If ReturnUrl is provided, use it. Otherwise, redirect to login page.
            var returnUrl = context.Request.Query["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
            {
                context.Response.Redirect("/Identity/Account/Login?returnUrl=" + returnUrl);
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

// Seed roles and admin user if necessary
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Create roles if they do not exist
    var roles = new[] { "Admin", "User" };
    foreach (var role in roles)
    {
        var roleExist = await roleManager.RoleExistsAsync(role);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Find the admin user by email
    var adminUser = await userManager.FindByEmailAsync("yllmurati19@gmail.com");

    if (adminUser != null)
    {
        // If the user exists, ensure they are in the Admin role
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
