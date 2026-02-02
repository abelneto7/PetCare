using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetCare.Web.Data;
using PetCare.Web.Models;
using PetCare.Web.Helpers;

namespace PetCare.Web.Controllers
{
    [Authorize]
    public class PetsController : Controller
    {
        private readonly AppDbContext _db;

        public PetsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var query = _db.Pets
                .AsNoTracking()
                .Include(p => p.Tutor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(p =>
                    p.Nome.Contains(q) ||
                    (p.Raca != null && p.Raca.Contains(q)) ||
                    (p.Tutor != null && p.Tutor.Nome.Contains(q)));
            }

            var itens = await query
                .OrderBy(p => p.Nome)
                .ToListAsync();

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

            var tutorExiste = await _db.Tutores.AnyAsync(t => t.Id == model.TutorId);
            if (!tutorExiste)
            {
                ModelState.AddModelError(nameof(Pet.TutorId), "Selecione um tutor válido.");
                TempData["Error"] = "Verifique os campos do formulário.";
                await CarregarCombosAsync(model.TutorId);
                return View(model);
            }

            _db.Pets.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Pet cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var pet = await _db.Pets.FindAsync(id);
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

            var pet = await _db.Pets.FindAsync(id);
            if (pet == null)
            {
                TempData["Error"] = "Pet não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var tutorExiste = await _db.Tutores.AnyAsync(t => t.Id == model.TutorId);
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

            await _db.SaveChangesAsync();

            TempData["Success"] = "Pet atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var pet = await _db.Pets
                .AsNoTracking()
                .Include(p => p.Tutor)
                .FirstOrDefaultAsync(p => p.Id == id);

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
            var pet = await _db.Pets.FindAsync(id);
            if (pet == null)
            {
                TempData["Error"] = "Pet não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            _db.Pets.Remove(pet);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Pet removido com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CarregarCombosAsync(int? tutorSelecionado = null)
        {
            var tutores = await _db.Tutores
                .AsNoTracking()
                .OrderBy(t => t.Nome)
                .Select(t => new { t.Id, t.Nome })
                .ToListAsync();

            ViewBag.Tutores = new SelectList(tutores, "Id", "Nome", tutorSelecionado);

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
