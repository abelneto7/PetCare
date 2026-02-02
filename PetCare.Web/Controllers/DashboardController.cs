using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.Domain.Interfaces;

namespace PetCare.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IPetRepository _petRepository;
        private readonly IVacinaRepository _vacinaRepository;

        public DashboardController(ITutorRepository tutorRepository, IPetRepository petRepository, IVacinaRepository vacinaRepository)
        {
            _tutorRepository = tutorRepository;
            _petRepository = petRepository;
            _vacinaRepository = vacinaRepository;
        }

        public async Task<IActionResult> Index()
        {
            var hoje = DateTime.UtcNow.Date;

            // Nota: Para otimização futura, adicionar métodos CountAsync nos repositórios.
            var tutores = await _tutorRepository.ObterTodosAsync();
            var pets = await _petRepository.ObterTodosAsync();
            var vacinasAtrasadasLista = await _vacinaRepository.ObterAtrasadasAsync();

            var vacinasAtrasadasVm = vacinasAtrasadasLista
                .Take(20) // Mantendo a lógica visual de limitar a 20
                .Select(v => new VacinaAtrasadaVm
                {
                    RegistroId = v.Id,
                    NomeVacina = v.NomeVacina,
                    ProximaDose = v.ProximaDose,
                    PetId = v.PetId,
                    PetNome = v.Pet!.Nome,
                    TutorNome = v.Pet!.Tutor != null ? v.Pet!.Tutor.Nome : "-"
                })
                .ToList();

            foreach (var item in vacinasAtrasadasVm)
            {
                item.DiasAtraso = (hoje - item.ProximaDose.Date).Days;
            }

            var vm = new DashboardVm
            {
                TotalTutores = tutores.Count,
                TotalPets = pets.Count,
                TotalVacinasAtrasadas = vacinasAtrasadasLista.Count,
                VacinasAtrasadas = vacinasAtrasadasVm
            };

            return View(vm);
        }

        public class DashboardVm
        {
            public int TotalTutores { get; set; }
            public int TotalPets { get; set; }
            public int TotalVacinasAtrasadas { get; set; }
            public List<VacinaAtrasadaVm> VacinasAtrasadas { get; set; } = new();
        }

        public class VacinaAtrasadaVm
        {
            public int RegistroId { get; set; }
            public string NomeVacina { get; set; } = "";
            public DateTime ProximaDose { get; set; }

            public int PetId { get; set; }
            public string PetNome { get; set; } = "";
            public string TutorNome { get; set; } = "";

            public int DiasAtraso { get; set; }
        }
    }
}
