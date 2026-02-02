using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Web.Data;
using PetCare.Web.Models;

namespace PetCare.Web.Controllers
{
    [Authorize]
    public class TutoresController : Controller
    {
        private readonly AppDbContext _db;

        public TutoresController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var query = _db.Tutores.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(t =>
                    t.Nome.Contains(q) ||
                    t.Telefone.Contains(q) ||
                    (t.Email != null && t.Email.Contains(q)));
            }

            var itens = await query
                .OrderBy(t => t.Nome)
                .ToListAsync();

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

            _db.Tutores.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Tutor cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tutor = await _db.Tutores.FindAsync(id);
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

            var tutor = await _db.Tutores.FindAsync(id);
            if (tutor == null)
            {
                TempData["Error"] = "Tutor não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            tutor.Nome = model.Nome;
            tutor.Telefone = model.Telefone;
            tutor.Email = model.Email;
            tutor.Endereco = model.Endereco;

            await _db.SaveChangesAsync();

            TempData["Success"] = "Tutor atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var tutor = await _db.Tutores.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
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
            var tutor = await _db.Tutores.FindAsync(id);
            if (tutor == null)
            {
                TempData["Error"] = "Tutor não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            _db.Tutores.Remove(tutor);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Tutor removido com sucesso.";
            return RedirectToAction(nameof(Index));
        }
    }
}
