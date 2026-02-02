using Microsoft.EntityFrameworkCore;
using PetCare.Domain.Entities;
using PetCare.Domain.Interfaces;
using PetCare.Infrastructure.Data;

namespace PetCare.Infrastructure.Repositories
{
    public class TutorRepository : ITutorRepository
    {
        private readonly AppDbContext _db;

        public TutorRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Tutor>> ObterTodosAsync(string? filtro = null)
        {
            var query = _db.Tutores.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.Trim();
                query = query.Where(t =>
                    t.Nome.Contains(filtro) ||
                    t.Telefone.Contains(filtro) ||
                    (t.Email != null && t.Email.Contains(filtro)));
            }

            return await query.OrderBy(t => t.Nome).ToListAsync();
        }

        public async Task<Tutor?> ObterPorIdAsync(int id)
        {
            return await _db.Tutores.FindAsync(id);
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _db.Tutores.AnyAsync(t => t.Id == id);
        }

        public async Task AdicionarAsync(Tutor tutor)
        {
            _db.Tutores.Add(tutor);
            await _db.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Tutor tutor)
        {
            _db.Tutores.Update(tutor);
            await _db.SaveChangesAsync();
        }

        public async Task RemoverAsync(Tutor tutor)
        {
            _db.Tutores.Remove(tutor);
            await _db.SaveChangesAsync();
        }
    }
}
