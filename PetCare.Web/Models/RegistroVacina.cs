using System.ComponentModel.DataAnnotations;

namespace PetCare.Web.Models
{
    public class RegistroVacina : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public int PetId { get; set; }

        public Pet? Pet { get; set; }

        [Required]
        [MaxLength(150)]
        public string NomeVacina { get; set; } = string.Empty;

        [Required]
        public DateTime DataAplicacao { get; set; }

        [Required]
        [Range(1, 3650)]
        public int IntervaloDias { get; set; }

        [Required]
        public DateTime ProximaDose { get; set; }

        [MaxLength(500)]
        public string? Observacao { get; set; }
    }
}
