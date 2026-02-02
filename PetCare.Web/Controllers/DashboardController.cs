using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Web.Data;

namespace PetCare.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var hoje = DateTime.UtcNow.Date;

            var totalTutores = await _db.Tutores.AsNoTracking().CountAsync();
            var totalPets = await _db.Pets.AsNoTracking().CountAsync();

            var totalVacinasAtrasadas = await _db.RegistrosVacinas
                .AsNoTracking()
                .CountAsync(v => v.ProximaDose < hoje);

            var vacinasAtrasadas = await _db.RegistrosVacinas
                .AsNoTracking()
                .Where(v => v.ProximaDose < hoje)
                .Include(v => v.Pet)
                .ThenInclude(p => p!.Tutor)
                .OrderBy(v => v.ProximaDose)
                .Take(20)
                .Select(v => new VacinaAtrasadaVm
                {
                    RegistroId = v.Id,
                    NomeVacina = v.NomeVacina,
                    ProximaDose = v.ProximaDose,
                    PetId = v.PetId,
                    PetNome = v.Pet!.Nome,
                    TutorNome = v.Pet!.Tutor != null ? v.Pet!.Tutor.Nome : "-"
                })
                .ToListAsync();

            foreach (var item in vacinasAtrasadas)
            {
                item.DiasAtraso = (hoje - item.ProximaDose.Date).Days;
            }

            var vm = new DashboardVm
            {
                TotalTutores = totalTutores,
                TotalPets = totalPets,
                TotalVacinasAtrasadas = totalVacinasAtrasadas,
                VacinasAtrasadas = vacinasAtrasadas
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
