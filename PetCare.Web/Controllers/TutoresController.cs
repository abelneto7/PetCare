using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.Domain.Entities;
using PetCare.Domain.Interfaces;

namespace PetCare.Web.Controllers
{
    [Authorize]
    public class TutoresController : Controller
    {
        private readonly ITutorRepository _tutorRepository;

        public TutoresController(ITutorRepository tutorRepository)
        {
            _tutorRepository = tutorRepository;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var itens = await _tutorRepository.ObterTodosAsync(q);
            ViewBag.Query = q;
            return View(itens);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Tutor());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tutor model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Verifique os campos do formulário.";
                return View(model);
            }

            await _tutorRepository.AdicionarAsync(model);

            TempData["Success"] = "Tutor cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tutor = await _tutorRepository.ObterPorIdAsync(id);
            if (tutor == null)
            {
                TempData["Error"] = "Tutor não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(tutor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tutor model)
        {
            if (id != model.Id)
            {
                TempData["Error"] = "Requisição inválida.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Verifique os campos do formulário.";
                return View(model);
            }

            var tutor = await _tutorRepository.ObterPorIdAsync(id);
            if (tutor == null)
            {
                TempData["Error"] = "Tutor não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            tutor.Nome = model.Nome;
            tutor.Telefone = model.Telefone;
            tutor.Email = model.Email;
            tutor.Endereco = model.Endereco;

            await _tutorRepository.AtualizarAsync(tutor);

            TempData["Success"] = "Tutor atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var tutor = await _tutorRepository.ObterPorIdAsync(id);
            if (tutor == null)
            {
                TempData["Error"] = "Tutor não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(tutor);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tutor = await _tutorRepository.ObterPorIdAsync(id);
            if (tutor == null)
            {
                TempData["Error"] = "Tutor não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            await _tutorRepository.RemoverAsync(tutor);

            TempData["Success"] = "Tutor removido com sucesso.";
            return RedirectToAction(nameof(Index));
        }
    }
}
