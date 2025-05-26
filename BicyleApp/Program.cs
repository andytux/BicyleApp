using BicyleApp.Components;
using BicyleApp.Data;
using BicyleApp.Data.Models;
using BicyleApp.Services;
using Microsoft.EntityFrameworkCore;
using static BicyleApp.Data.Enums.Enum;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<CompetitionService>();
builder.Services.AddScoped<RankingService>();
builder.Services.AddScoped<RideService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userService = scope.ServiceProvider.GetRequiredService<UserService>();

    var existingAdmin = await userService.GetByEmailAsync("admin@admin.at");
    if (existingAdmin == null)
    {
        var adminUser = new User
        {
            Email = "admin@admin.at",
            Name = "Admin",
            City = "Adminstadt",
            Role = UserRole.Admin,
            IsAdmin = true,
            PasswordHash = AuthService.HashPassword("Admin12345")
        };

        await userService.AddAsync(adminUser);

        Console.WriteLine("Admin-User wurde erstellt.");
    }
}
await SeedTestDataAsync(app);

app.Run();


// GANZ UNTEN in Program.cs:

static async Task SeedTestDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userService = scope.ServiceProvider.GetRequiredService<UserService>();
    var compService = scope.ServiceProvider.GetRequiredService<CompetitionService>();
    var rideService = scope.ServiceProvider.GetRequiredService<RideService>();

    var usersToCreate = new (string Email, string Pwd, string Name, string City, UserRole Role)[]
    {
        ("admin@admin.at",  "Admin12345",   "Admin",     "Adminstadt", UserRole.Admin),
        ("alice@demo.at",   "Passwort123", "Alice",     "Wien",       UserRole.User),
        ("bob@demo.at",     "Passwort123", "Bob",       "Graz",       UserRole.User),
        ("carol@demo.at",   "Passwort123", "Carol",     "Linz",       UserRole.User),
        ("dave@demo.at",    "Passwort123", "Dave",      "Salzburg",   UserRole.User)
    };

    var userEntities = new List<User>();

    foreach (var (email, pwd, name, city, role) in usersToCreate)
    {
        var u = await userService.GetByEmailAsync(email);
        if (u == null)
        {
            u = new User
            {
                Email = email,
                Name = name,
                City = city,
                Role = role,
                IsAdmin = role == UserRole.Admin,
                PasswordHash = AuthService.HashPassword(pwd)
            };
            await userService.AddAsync(u);
        }
        userEntities.Add(u!);
    }

    var compsToCreate = new[]
    {
        new Competition
        {
            Title     = "Spring Ride 2024",
            StartDate = DateTime.UtcNow.Date.AddDays(-10),
            EndDate   = DateTime.UtcNow.Date.AddDays(+20),
            Status    = CompetitionStatus.Active
        },
        new Competition
        {
            Title     = "Summer Challenge 2024",
            StartDate = DateTime.UtcNow.Date.AddDays(+30),
            EndDate   = DateTime.UtcNow.Date.AddDays(+60),
            Status    = CompetitionStatus.Inactive
        },
        new Competition
        {
            Title     = "Winter Warm-Up 2023",
            StartDate = new DateTime(2023,11,1),
            EndDate   = new DateTime(2023,11,30),
            Status    = CompetitionStatus.Inactive
        }
    };

    foreach (var c in compsToCreate)
    {
        var exists = await db.Competitions.AnyAsync(x => x.Title == c.Title);
        if (!exists)
        {
            db.Competitions.Add(c);
        }
    }
    await db.SaveChangesAsync();

    var activeComp = await compService.GetActiveCompetitionAsync();
    if (activeComp == null) return;

    var rnd = new Random(1);
    foreach (var user in userEntities.Where(u => u.Role == UserRole.User))
    {
        for (var i = 0; i < 7; i++)
        {
            var rideDate = DateTime.UtcNow.Date.AddDays(-i);

            if (await rideService.RideExistsOnDateAsync(user.UserId, rideDate))
                continue;

            var ride = new Ride
            {
                UserId = user.UserId,
                CompetitionId = activeComp.CompetitionId,
                Date = rideDate,
                DistanceKm = rnd.Next(5, 45)
            };
            await rideService.AddAsync(ride);
        }
    }

    Console.WriteLine("Seed-Daten erfolgreich eingefügt.");
}
