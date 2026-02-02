using Microsoft.AspNetCore.Identity;
using PetCare.Application.Interfaces;
using PetCare.Domain.Entities;
using PetCare.Domain.Interfaces;

namespace PetCare.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public AuthService(IUsuarioRepository usuarioRepository, IPasswordHasher<Usuario> passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<(bool Sucesso, string Mensagem, Usuario? Usuario)> RegisterAsync(string nome, string email, string senha)
        {
            if (await _usuarioRepository.ExisteEmailAsync(email))
                return (false, "Já existe um usuário com este email.", null);

            var usuario = new Usuario
            {
                Nome = nome,
                Email = email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            usuario.Password = _passwordHasher.HashPassword(usuario, senha);

            await _usuarioRepository.AdicionarAsync(usuario);

            return (true, "Conta criada com sucesso.", usuario);
        }

        public async Task<(bool Sucesso, string Mensagem, Usuario? Usuario)> LoginAsync(string email, string senha)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
            if (usuario == null)
                return (false, "Email ou senha inválidos.", null);

            var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Password, senha);
            if (result == PasswordVerificationResult.Failed)
                return (false, "Email ou senha inválidos.", null);

            return (true, "Login realizado com sucesso.", usuario);
        }
    }
}
