using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetCare.Application.Services;
using PetCare.Domain.Entities;
using PetCare.Domain.Enums;
using PetCare.Domain.Interfaces;

namespace PetCare.Web.Controllers
{
    [Authorize]
    public class VacinasController : Controller
    {
        private readonly IVacinaRepository _vacinaRepository;
        private readonly IPetRepository _petRepository;
        private readonly VacinaService _vacinaService;

        public VacinasController(IVacinaRepository vacinaRepository, IPetRepository petRepository, VacinaService vacinaService)
        {
            _vacinaRepository = vacinaRepository;
            _petRepository = petRepository;
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

            var petExiste = await _petRepository.ExisteAsync(model.PetId);
            if (!petExiste)
            {
                ModelState.AddModelError(nameof(RegistroVacina.PetId), "Selecione um pet válido.");
                TempData["Error"] = "Verifique os campos do formulário.";
                await CarregarPetsAsync(model.PetId);
                return View(model);
            }

            model.ProximaDose = _vacinaService.CalcularProximaDose(model.DataAplicacao, model.IntervaloDias);

            await _vacinaRepository.AdicionarAsync(model);

            TempData["Success"] = "Vacina registrada com sucesso.";
            return RedirectToAction(nameof(PorPet), new { id = model.PetId });
        }

        [HttpGet("/pets/{id:int}/vacinas")]
        public async Task<IActionResult> PorPet(int id)
        {
            var pet = await _petRepository.ObterPorIdAsync(id);

            if (pet == null)
            {
                TempData["Error"] = "Pet não encontrado.";
                return RedirectToAction("Index", "Pets");
            }

            var vacinas = await _vacinaRepository.ObterPorPetAsync(id);

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
            var vacina = await _vacinaRepository.ObterPorIdAsync(id);
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

            var vacina = await _vacinaRepository.ObterPorIdAsync(id);
            if (vacina == null)
            {
                TempData["Error"] = "Registro de vacina não encontrado.";
                return RedirectToAction("Index", "Pets");
            }

            var petExiste = await _petRepository.ExisteAsync(model.PetId);
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

            await _vacinaRepository.AtualizarAsync(vacina);

            TempData["Success"] = "Registro de vacina atualizado com sucesso.";
            return RedirectToAction(nameof(PorPet), new { id = vacina.PetId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var vacina = await _vacinaRepository.ObterPorIdAsync(id);

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
            var vacina = await _vacinaRepository.ObterPorIdAsync(id);
            if (vacina == null)
            {
                TempData["Error"] = "Registro de vacina não encontrado.";
                return RedirectToAction("Index", "Pets");
            }

            var petId = vacina.PetId;

            await _vacinaRepository.RemoverAsync(vacina);

            TempData["Success"] = "Registro de vacina removido com sucesso.";
            return RedirectToAction(nameof(PorPet), new { id = petId });
        }

        private async Task CarregarPetsAsync(int? petSelecionado = null)
        {
            var pets = await _petRepository.ObterTodosAsync();
            
            var lista = pets
                .Select(p => new
                {
                    p.Id,
                    Texto = p.Tutor != null ? (p.Nome + " - " + p.Tutor.Nome) : p.Nome
                })
                .OrderBy(x => x.Texto)
                .ToList();

            ViewBag.Pets = new SelectList(lista, "Id", "Texto", petSelecionado);
        }

        public class VacinaLinhaVm
        {
            public RegistroVacina Vacina { get; set; } = default!;
            public StatusVacina Status { get; set; }
        }
    }
}
