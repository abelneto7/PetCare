using PetCare.Domain.Entities;

namespace PetCare.Domain.Interfaces
{
    public interface IVacinaRepository
    {
        Task<List<RegistroVacina>> ObterPorPetAsync(int petId);
        Task<List<RegistroVacina>> ObterAtrasadasAsync();
        Task<RegistroVacina?> ObterPorIdAsync(int id);
        Task AdicionarAsync(RegistroVacina vacina);
        Task AtualizarAsync(RegistroVacina vacina);
        Task RemoverAsync(RegistroVacina vacina);
    }
}
