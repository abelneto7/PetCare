using Microsoft.EntityFrameworkCore;
using PetCare.Domain.Entities;
using PetCare.Domain.Interfaces;
using PetCare.Infrastructure.Data;

namespace PetCare.Infrastructure.Repositories
{
    public class VacinaRepository : IVacinaRepository
    {
        private readonly AppDbContext _db;

        public VacinaRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<RegistroVacina>> ObterPorPetAsync(int petId)
        {
            return await _db.RegistrosVacinas
                .AsNoTracking()
                .Where(v => v.PetId == petId)
                .OrderByDescending(v => v.DataAplicacao)
                .ToListAsync();
        }

        public async Task<List<RegistroVacina>> ObterAtrasadasAsync()
        {
            var hoje = DateTime.UtcNow.Date;
            
            return await _db.RegistrosVacinas
                .AsNoTracking()
                .Where(v => v.ProximaDose < hoje)
                .Include(v => v.Pet)
                .ThenInclude(p => p!.Tutor)
                .OrderBy(v => v.ProximaDose)
                .ToListAsync();
        }

        public async Task<RegistroVacina?> ObterPorIdAsync(int id)
        {
            return await _db.RegistrosVacinas
                .Include(v => v.Pet)
                .ThenInclude(p => p!.Tutor)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task AdicionarAsync(RegistroVacina vacina)
        {
            _db.RegistrosVacinas.Add(vacina);
            await _db.SaveChangesAsync();
        }

        public async Task AtualizarAsync(RegistroVacina vacina)
        {
            _db.RegistrosVacinas.Update(vacina);
            await _db.SaveChangesAsync();
        }

        public async Task RemoverAsync(RegistroVacina vacina)
        {
            _db.RegistrosVacinas.Remove(vacina);
            await _db.SaveChangesAsync();
        }
    }
}
