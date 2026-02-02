using System.ComponentModel.DataAnnotations;

namespace PetCare.Web.Models
{
    public class Tutor : BaseEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome do tutor.")]
        [MaxLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o telefone.")]
        [MaxLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres.")]
        public string Telefone { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "O email deve ter no máximo 200 caracteres.")]
        [EmailAddress(ErrorMessage = "Informe um email válido.")]
        public string? Email { get; set; }

        [MaxLength(300, ErrorMessage = "O endereço deve ter no máximo 300 caracteres.")]
        public string? Endereco { get; set; }
    }
}
