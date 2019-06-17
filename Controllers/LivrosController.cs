using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Data;
using Biblioteca.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;

namespace Biblioteca.Controllers
{
    [Authorize]
    public class LivrosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LivrosController(ApplicationDbContext context)
        {
            _context = context;
        }
        public void LoadViewBags()
        {
            ViewBag.Users = _context.Users;
        }
        // GET: Livros
        public async Task<IActionResult> Index()
        {
            LivrosIndexViewModel ViewModel = new LivrosIndexViewModel();
            ViewModel.UserIdLogado = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewModel.Livros = await Filter(ViewModel);
            ViewData["error"] = TempData["error"];
            ViewData["success"] = TempData["success"];
            LoadViewBags();
            return View(ViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(LivrosIndexViewModel ViewModel)
        {
            ViewModel.Livros = await Filter(ViewModel);
            LoadViewBags();
            return View(ViewModel);
        }
        public async Task<IEnumerable<Livro>> Filter(LivrosIndexViewModel ViewModel)
        {
            return await _context.Livro
                .Where(l => l.Nome.Contains(ViewModel.Nome) ||
                l.Autor.Contains(ViewModel.Nome) || 
                l.Ano.ToString().Contains(ViewModel.Nome) ||
                string.IsNullOrEmpty(ViewModel.Nome))
                .Where(l => l.UserId == ViewModel.UserId || ViewModel.UserId == null)
                .OrderBy(l => l.Id).Include(l => l.User)
                .ToListAsync();
        }

        // GET: Livros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            var livro = await _context.Livro.Include(l => l.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livro == null)
                return NotFound();
            ViewBag.UserIdLogado = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return View(livro);
        }

        // GET: Livros/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Livros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Autor,Ano")] Livro livro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(livro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(livro);
        }

        // GET: Livros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var livro = await _context.Livro.FindAsync(id);
            if (livro == null)
                return NotFound();

            if (livro.UserId != null)
            {
                TempData["error"] = "Este livro não pode ser editado pois está alugado.";
                return RedirectToAction(nameof(Index));
            }
            else
                return View(livro);
        }

        // POST: Livros/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Autor,Ano")] Livro livro)
        {
            if (id != livro.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    if (livro.UserId != null)
                    {
                        TempData["error"] = "Este livro não pode ser editado pois está alugado.";
                        return RedirectToAction(nameof(Index));
                    }
                    _context.Update(livro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LivroExists(livro.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(livro);
        }

        // GET: Livros/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var livro = await _context.Livro
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livro == null)
                return NotFound();
            if (livro.UserId != null)
            {
                TempData["error"] = "Este livro não pode ser excluido pois está alugado.";
                return RedirectToAction(nameof(Index));
            }
            return View(livro);
        }

        // POST: Livros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var livro = await _context.Livro.FindAsync(id);

            if (livro.UserId != null)
            {
                TempData["error"] = "Este livro não pode ser excluido pois está alugado.";
                return RedirectToAction(nameof(Index));
            }
            _context.Livro.Remove(livro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LivroExists(int id)
        {
            return _context.Livro.Any(e => e.Id == id);
        }

        // GET: Livros/Rent/5
        [HttpGet]
        public async Task<IActionResult> Rent(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var livro = await _context.Livro.FindAsync(id);
                    if (livro.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                        TempData["error"] = "este livro já alugado por você.";
                    if (livro.UserId != null)
                        TempData["error"] = "Este livro já está alugado.";
                    else
                    {
                        TempData["success"] = "Parabéns! você alugou o livro " + livro.Nome;
                        livro.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }
            return RedirectToAction(nameof(Index));
        }
        // GET: Livros/Rent/5
        [HttpGet]
        public async Task<IActionResult> GiveBack(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var livro = await _context.Livro.FindAsync(id);
                    livro.UserId = null;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
