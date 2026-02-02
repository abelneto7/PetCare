using Microsoft.EntityFrameworkCore;
using PetCare.Domain.Entities;
using PetCare.Domain.Interfaces;
using PetCare.Infrastructure.Data;

namespace PetCare.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _db;

        public UsuarioRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            return await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            return await _db.Usuarios.AnyAsync(u => u.Email == email);
        }

        public async Task AdicionarAsync(Usuario usuario)
        {
            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();
        }
    }
}
