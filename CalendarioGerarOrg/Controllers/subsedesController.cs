using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CalendarioGerarOrg.Models;

namespace CalendarioGerarOrg.Controllers
{
    public class subsedesController : Controller
    {
        private readonly AppDbContext _context;

        public subsedesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: subsedes
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Subsede.Include(s => s.idcidadeNavigation);
            return View(await appDbContext.ToListAsync());
        }

        // GET: subsedes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subsede = await _context.Subsede
                .Include(s => s.idcidadeNavigation)
                .FirstOrDefaultAsync(m => m.idsubsede == id);
            if (subsede == null)
            {
                return NotFound();
            }

            return View(subsede);
        }

        // GET: subsedes/Create
        public IActionResult Create()
        {
            ViewData["idcidade"] = new SelectList(_context.Cidade, "idcidade", "nome");
            return View();
        }

        // POST: subsedes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("idsubsede,idcidade,nome")] subsede subsede)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subsede);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["idcidade"] = new SelectList(_context.Cidade, "idcidade", "nome", subsede.idcidade);
            return View(subsede);
        }

        // GET: subsedes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subsede = await _context.Subsede.FindAsync(id);
            if (subsede == null)
            {
                return NotFound();
            }
            ViewData["idcidade"] = new SelectList(_context.Cidade, "idcidade", "nome", subsede.idcidade);
            return View(subsede);
        }

        // POST: subsedes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idsubsede,idcidade,nome")] subsede subsede)
        {
            if (id != subsede.idsubsede)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subsede);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!subsedeExists(subsede.idsubsede))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["idcidade"] = new SelectList(_context.Cidade, "idcidade", "nome", subsede.idcidade);
            return View(subsede);
        }

        // GET: subsedes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subsede = await _context.Subsede
                .Include(s => s.idcidadeNavigation)
                .FirstOrDefaultAsync(m => m.idsubsede == id);
            if (subsede == null)
            {
                return NotFound();
            }

            return View(subsede);
        }

        // POST: subsedes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subsede = await _context.Subsede.FindAsync(id);
            _context.Subsede.Remove(subsede);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool subsedeExists(int id)
        {
            return _context.Subsede.Any(e => e.idsubsede == id);
        }
    }
}
