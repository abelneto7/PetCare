using System.ComponentModel.DataAnnotations;
using PetCare.Domain.Enums;

namespace PetCare.Domain.Entities
{
    public class Pet : BaseEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome do pet.")]
        [MaxLength(150, ErrorMessage = "O nome do pet deve ter no máximo 150 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a espécie.")]
        public EspeciePet Especie { get; set; } = EspeciePet.NaoInformado;

        [MaxLength(150, ErrorMessage = "A raça deve ter no máximo 150 caracteres.")]
        public string? Raca { get; set; }

        [Required(ErrorMessage = "Selecione um tutor.")]
        public int TutorId { get; set; }

        public Tutor? Tutor { get; set; }

        public ICollection<RegistroVacina> Vacinas { get; set; } = new List<RegistroVacina>();
    }
}
