using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetCare.Domain.Entities;
using PetCare.Domain.Enums;
using PetCare.Domain.Interfaces;
using PetCare.Web.Helpers;

namespace PetCare.Web.Controllers
{
    [Authorize]
    public class PetsController : Controller
    {
        private readonly IPetRepository _petRepository;
        private readonly ITutorRepository _tutorRepository;

        public PetsController(IPetRepository petRepository, ITutorRepository tutorRepository)
        {
            _petRepository = petRepository;
            _tutorRepository = tutorRepository;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var itens = await _petRepository.ObterTodosAsync(q);
            ViewBag.Query = q;
            return View(itens);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CarregarCombosAsync();
            return View(new Pet { Especie = EspeciePet.NaoInformado });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pet model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Verifique os campos do formulário.";
                await CarregarCombosAsync(model.TutorId);
                return View(model);
            }

            var tutorExiste = await _tutorRepository.ExisteAsync(model.TutorId);
            if (!tutorExiste)
            {
                ModelState.AddModelError(nameof(Pet.TutorId), "Selecione um tutor válido.");
                TempData["Error"] = "Verifique os campos do formulário.";
                await CarregarCombosAsync(model.TutorId);
                return View(model);
            }

            await _petRepository.AdicionarAsync(model);

            TempData["Success"] = "Pet cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var pet = await _petRepository.ObterPorIdAsync(id);
            if (pet == null)
            {
                TempData["Error"] = "Pet não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            await CarregarCombosAsync(pet.TutorId);
            return View(pet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pet model)
        {
            if (id != model.Id)
            {
                TempData["Error"] = "Requisição inválida.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Verifique os campos do formulário.";
                await CarregarCombosAsync(model.TutorId);
                return View(model);
            }

            var pet = await _petRepository.ObterPorIdAsync(id);
            if (pet == null)
            {
                TempData["Error"] = "Pet não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var tutorExiste = await _tutorRepository.ExisteAsync(model.TutorId);
            if (!tutorExiste)
            {
                ModelState.AddModelError(nameof(Pet.TutorId), "Selecione um tutor válido.");
                TempData["Error"] = "Verifique os campos do formulário.";
                await CarregarCombosAsync(model.TutorId);
                return View(model);
            }

            pet.Nome = model.Nome;
            pet.Especie = model.Especie;
            pet.Raca = model.Raca;
            pet.TutorId = model.TutorId;

            await _petRepository.AtualizarAsync(pet);

            TempData["Success"] = "Pet atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var pet = await _petRepository.ObterPorIdAsync(id);

            if (pet == null)
            {
                TempData["Error"] = "Pet não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(pet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pet = await _petRepository.ObterPorIdAsync(id);
            if (pet == null)
            {
                TempData["Error"] = "Pet não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            await _petRepository.RemoverAsync(pet);

            TempData["Success"] = "Pet removido com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CarregarCombosAsync(int? tutorSelecionado = null)
        {
            var tutores = await _tutorRepository.ObterTodosAsync();

            var lista = tutores
                .OrderBy(t => t.Nome)
                .Select(t => new { t.Id, t.Nome })
                .ToList();

            ViewBag.Tutores = new SelectList(lista, "Id", "Nome", tutorSelecionado);

            var especies = Enum.GetValues(typeof(EspeciePet))
                .Cast<EspeciePet>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToDisplay()
                })
                .ToList();

            ViewBag.Especies = especies;
        }
    }
}
