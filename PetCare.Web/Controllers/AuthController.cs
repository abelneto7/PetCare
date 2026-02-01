using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PetCare.Web.Data;
using PetCare.Web.Models;
using System.Security.Claims;

namespace PetCare.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public AuthController(AppDbContext db, IPasswordHasher<Usuario> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastro(string nome, string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                ViewBag.Erro = "Preencha nome, email e senha.";
                return View();
            }

            email = email.Trim().ToLowerInvariant();

            var existe = await _db.Usuarios.AnyAsync(u => u.Email == email);
            if (existe)
            {
                ViewBag.Erro = "Já existe um usuário com este email.";
                return View();
            }

            var usuario = new Usuario
            {
                Nome = nome.Trim(),
                Email = email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            usuario.Password = _passwordHasher.HashPassword(usuario, senha);

            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Conta criada com sucesso. Agora você pode entrar.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                ViewBag.Erro = "Informe email e senha.";
                return View();
            }

            email = email.Trim().ToLowerInvariant();

            var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuario is null)
            {
                ViewBag.Erro = "Email ou senha inválidos.";
                return View();
            }

            var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Password, senha);
            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Erro = "Email ou senha inválidos.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    IssuedUtc = DateTimeOffset.UtcNow
                });

            TempData["Success"] = $"Bem-vindo, {usuario.Nome}!";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Info"] = "Você saiu do sistema.";
            return RedirectToAction(nameof(Login));
        }
    }
}
