using FitnessApp.Data;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using Microsoft.AspNetCore.Identity;
=======
>>>>>>> 101432024641ce337e528306d3ec0ee1ec850162

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

<<<<<<< HEAD
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
});

=======
// Add DbContext with PostgreSQL provider
>>>>>>> 101432024641ce337e528306d3ec0ee1ec850162
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


<<<<<<< HEAD
// -------------------------------
//  ROL & ADMIN OLUŞTURMA METODLARI
// -------------------------------
async Task CreateDefaultRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "Uye" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

async Task CreateDefaultAdmin(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string adminEmail = "G231210385@sakarya.edu.tr";
    string adminPassword = "Sau123!";

    // Admin kullanıcı var mı?
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);

        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else
        {
            foreach (var err in createResult.Errors)
                Console.WriteLine("ADMIN ERROR: " + err.Description);
        }
    }
}


// -------------------------------
//  UYGULAMAYI BAŞLAT
// -------------------------------
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
=======

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
>>>>>>> 101432024641ce337e528306d3ec0ee1ec850162
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

<<<<<<< HEAD
app.UseAuthentication();
app.UseAuthorization();

// -------------------------------
//  TEK SCOPE - ÇALIŞMASI GEREKEN YER
// -------------------------------
using (var scope = app.Services.CreateScope())
{
    await CreateDefaultRoles(scope.ServiceProvider);
    await CreateDefaultAdmin(scope.ServiceProvider);
}

=======
app.UseAuthorization();

>>>>>>> 101432024641ce337e528306d3ec0ee1ec850162
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
