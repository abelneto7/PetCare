using PetCare.Domain.Entities;

namespace PetCare.Domain.Interfaces
{
    public interface ITutorRepository
    {
        Task<List<Tutor>> ObterTodosAsync(string? filtro = null);
        Task<Tutor?> ObterPorIdAsync(int id);
        Task<bool> ExisteAsync(int id);
        Task AdicionarAsync(Tutor tutor);
        Task AtualizarAsync(Tutor tutor);
        Task RemoverAsync(Tutor tutor);
    }
}
