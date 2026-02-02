using PetCare.Domain.Entities;

namespace PetCare.Domain.Interfaces
{
    public interface IPetRepository
    {
        Task<List<Pet>> ObterTodosAsync(string? filtro = null);
        Task<Pet?> ObterPorIdAsync(int id);
        Task<bool> ExisteAsync(int id);
        Task AdicionarAsync(Pet pet);
        Task AtualizarAsync(Pet pet);
        Task RemoverAsync(Pet pet);
    }
}
