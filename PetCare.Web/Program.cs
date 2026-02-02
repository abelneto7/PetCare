using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PetCare.Infrastructure.Data;
using PetCare.Domain.Entities;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));



builder.Services.AddScoped<PetCare.Application.Interfaces.IAuthService, PetCare.Application.Services.AuthService>();
builder.Services.AddScoped<PetCare.Application.Services.VacinaService>();

builder.Services.AddScoped<PetCare.Domain.Interfaces.IUsuarioRepository, PetCare.Infrastructure.Repositories.UsuarioRepository>();
builder.Services.AddScoped<PetCare.Domain.Interfaces.ITutorRepository, PetCare.Infrastructure.Repositories.TutorRepository>();
builder.Services.AddScoped<PetCare.Domain.Interfaces.IPetRepository, PetCare.Infrastructure.Repositories.PetRepository>();
builder.Services.AddScoped<PetCare.Domain.Interfaces.IVacinaRepository, PetCare.Infrastructure.Repositories.VacinaRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
