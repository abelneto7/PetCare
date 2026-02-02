using PetCare.Domain.Entities;

namespace PetCare.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Sucesso, string Mensagem, Usuario? Usuario)> RegisterAsync(string nome, string email, string senha);
        Task<(bool Sucesso, string Mensagem, Usuario? Usuario)> LoginAsync(string email, string senha);
    }
}
