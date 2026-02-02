using PetCare.Domain.Entities;
using PetCare.Domain.Enums;

namespace PetCare.Application.Services
{
    public class VacinaService
    {
        public DateTime CalcularProximaDose(DateTime dataAplicacao, int intervaloDias)
        {
            if (intervaloDias <= 0)
                throw new ArgumentOutOfRangeException(nameof(intervaloDias), "Intervalo deve ser maior que zero.");

            return dataAplicacao.Date.AddDays(intervaloDias);
        }

        public StatusVacina ObterStatus(DateTime proximaDose, DateTime? hojeUtc = null)
        {
            var hoje = (hojeUtc ?? DateTime.UtcNow).Date;
            var vencimento = proximaDose.Date;

            if (vencimento < hoje)
                return StatusVacina.Atrasada;

            if (vencimento <= hoje.AddDays(7))
                return StatusVacina.VenceEmBreve;

            return StatusVacina.EmDia;
        }
    }
}
