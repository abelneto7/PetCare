using PetCare.Domain.Entities;

namespace PetCare.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<bool> ExisteEmailAsync(string email);
        Task AdicionarAsync(Usuario usuario);
    }
}
