using Microsoft.EntityFrameworkCore;
using PetCare.Domain.Entities;
using PetCare.Domain.Interfaces;
using PetCare.Infrastructure.Data;

namespace PetCare.Infrastructure.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly AppDbContext _db;

        public PetRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Pet>> ObterTodosAsync(string? filtro = null)
        {
            var query = _db.Pets
                .AsNoTracking()
                .Include(p => p.Tutor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.Trim();
                query = query.Where(p =>
                    p.Nome.Contains(filtro) ||
                    (p.Raca != null && p.Raca.Contains(filtro)) ||
                    (p.Tutor != null && p.Tutor.Nome.Contains(filtro)));
            }

            return await query.OrderBy(p => p.Nome).ToListAsync();
        }

        public async Task<Pet?> ObterPorIdAsync(int id)
        {
            return await _db.Pets
                .Include(p => p.Tutor)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _db.Pets.AnyAsync(p => p.Id == id);
        }

        public async Task AdicionarAsync(Pet pet)
        {
            _db.Pets.Add(pet);
            await _db.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Pet pet)
        {
            _db.Pets.Update(pet);
            await _db.SaveChangesAsync();
        }

        public async Task RemoverAsync(Pet pet)
        {
            _db.Pets.Remove(pet);
            await _db.SaveChangesAsync();
        }
    }
}
