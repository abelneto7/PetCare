using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetCare.Web.Data;
using PetCare.Web.Models;
using PetCare.Web.Services;

namespace PetCare.Web.Controllers
{
    [Authorize]
    public class VacinasController : Controller
    {
        private readonly AppDbContext _db;
        private readonly VacinaService _vacinaService;

        public VacinasController(AppDbContext db, VacinaService vacinaService)
        {
            _db = db;
            _vacinaService = vacinaService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? petId)
        {
            await CarregarPetsAsync(petId);

            var model = new RegistroVacina
            {
                PetId = petId ?? 0,
                DataAplicacao = DateTime.UtcNow.Date,
                IntervaloDias = 365
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistroVacina model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Verifique os campos do formulário.";
                await CarregarPetsAsync(model.PetId);
                return View(model);
            }

            var petExiste = await _db.Pets.AnyAsync(p => p.Id == model.PetId);
            if (!petExiste)
            {
                ModelState.AddModelError(nameof(RegistroVacina.PetId), "Selecione um pet válido.");
                TempData["Error"] = "Verifique os campos do formulário.";
                await CarregarPetsAsync(model.PetId);
                return View(model);
            }

            model.ProximaDose = _vacinaService.CalcularProximaDose(model.DataAplicacao, model.IntervaloDias);

            _db.RegistrosVacinas.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Vacina registrada com sucesso.";
            return RedirectToAction(nameof(PorPet), new { id = model.PetId });
        }

        [HttpGet("/pets/{id:int}/vacinas")]
        public async Task<IActionResult> PorPet(int id)
        {
            var pet = await _db.Pets
                .AsNoTracking()
                .Include(p => p.Tutor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pet == null)
            {
                TempData["Error"] = "Pet não encontrado.";
                return RedirectToAction("Index", "Pets");
            }

            var vacinas = await _db.RegistrosVacinas
                .AsNoTracking()
                .Where(v => v.PetId == id)
                .OrderByDescending(v => v.DataAplicacao)
                .ToListAsync();

            var itens = vacinas.Select(v => new VacinaLinhaVm
            {
                Vacina = v,
                Status = _vacinaService.ObterStatus(v.ProximaDose)
            }).ToList();

            ViewBag.Pet = pet;
            return View(itens);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vacina = await _db.RegistrosVacinas.FindAsync(id);
            if (vacina == null)
            {
                TempData["Error"] = "Registro de vacina não encontrado.";
                return RedirectToAction("Index", "Pets");
            }

            await CarregarPetsAsync(vacina.PetId);
            return View(vacina);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RegistroVacina model)
        {
            if (id != model.Id)
            {
                TempData["Error"] = "Requisição inválida.";
                return RedirectToAction("Index", "Pets");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Verifique os campos do formulário.";
                await CarregarPetsAsync(model.PetId);
                return View(model);
            }

            var vacina = await _db.RegistrosVacinas.FindAsync(id);
            if (vacina == null)
            {
                TempData["Error"] = "Registro de vacina não encontrado.";
                return RedirectToAction("Index", "Pets");
            }

            var petExiste = await _db.Pets.AnyAsync(p => p.Id == model.PetId);
            if (!petExiste)
            {
                ModelState.AddModelError(nameof(RegistroVacina.PetId), "Selecione um pet válido.");
                TempData["Error"] = "Verifique os campos do formulário.";
                await CarregarPetsAsync(model.PetId);
                return View(model);
            }

            vacina.PetId = model.PetId;
            vacina.NomeVacina = model.NomeVacina;
            vacina.DataAplicacao = model.DataAplicacao;
            vacina.IntervaloDias = model.IntervaloDias;
            vacina.Observacao = model.Observacao;

            vacina.ProximaDose = _vacinaService.CalcularProximaDose(vacina.DataAplicacao, vacina.IntervaloDias);

            await _db.SaveChangesAsync();

            TempData["Success"] = "Registro de vacina atualizado com sucesso.";
            return RedirectToAction(nameof(PorPet), new { id = vacina.PetId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var vacina = await _db.RegistrosVacinas
                .AsNoTracking()
                .Include(v => v.Pet)
                .ThenInclude(p => p!.Tutor)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vacina == null)
            {
                TempData["Error"] = "Registro de vacina não encontrado.";
                return RedirectToAction("Index", "Pets");
            }

            return View(vacina);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vacina = await _db.RegistrosVacinas.FindAsync(id);
            if (vacina == null)
            {
                TempData["Error"] = "Registro de vacina não encontrado.";
                return RedirectToAction("Index", "Pets");
            }

            var petId = vacina.PetId;

            _db.RegistrosVacinas.Remove(vacina);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Registro de vacina removido com sucesso.";
            return RedirectToAction(nameof(PorPet), new { id = petId });
        }

        private async Task CarregarPetsAsync(int? petSelecionado = null)
        {
            var pets = await _db.Pets
                .AsNoTracking()
                .Include(p => p.Tutor)
                .OrderBy(p => p.Nome)
                .Select(p => new
                {
                    p.Id,
                    Texto = p.Tutor != null ? (p.Nome + " — " + p.Tutor.Nome) : p.Nome
                })
                .ToListAsync();

            ViewBag.Pets = new SelectList(pets, "Id", "Texto", petSelecionado);
        }

        public class VacinaLinhaVm
        {
            public RegistroVacina Vacina { get; set; } = default!;
            public StatusVacina Status { get; set; }
        }
    }
}
