using System.ComponentModel.DataAnnotations;

namespace PetCare.Web.Models
{
    public enum EspeciePet
    {
        [Display(Name = "Não informado")]
        NaoInformado = 0,

        Cachorro = 1,
        Gato = 2,
        Hamster = 3,
        Coelho = 4,

        [Display(Name = "Pássaro")]
        Passaro = 5,

        Peixe = 6,
        Tartaruga = 7,

        [Display(Name = "Porquinho da Índia")]
        PorquinhoDaIndia = 8,

        [Display(Name = "Furão")]
        Furao = 9,

        Lagarto = 10,
        Cobra = 11,

        Outro = 99
    }
}
