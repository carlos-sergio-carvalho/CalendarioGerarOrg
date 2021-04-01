using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CalendarioGerarOrg.Models;
using CalendarioGerarOrg.ViewModels;
using Microsoft.Extensions.Configuration;

namespace CalendarioGerarOrg.Controllers
{
    public class feriadosController : Controller
    {
        private readonly AppDbContext _context;

        public feriadosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: feriados
        public async Task<IActionResult> Index([FromServices] IConfiguration config)
        {
            //var appDbContext = _context.Feriado.Include(f => f.idcidadeNavigation);
            //var ca = new cidadeAno();

            //ca.cidades = await  _context.Cidade.OrderBy(p => p.nome).ToListAsync();
            var anos = await _context.Feriado.Select(p => p.dia.Year).Distinct().OrderBy(p=>p).ToListAsync();
            anos.Add(anos.Max() + 1);
            anos.Add(anos.Max() + 1);
            var cidades = await _context.Cidade.OrderBy(p => p.nome).ToListAsync();
            if(!config.GetValue<bool>("ShowGeral"))
            {
                var geral = cidades.Single(p => p.idcidade == 0);
                cidades.Remove(geral);
                //geral.nome = "";
            }
            cidades.Insert(0, new cidade() { idcidade = -1, nome = "" });
            ViewData["cidade"] = new SelectList(cidades, "idcidade", "nome",-1 );
            ViewData["ano"] = new SelectList(anos, DateTime.Now.Year);
            return View();
        }

        public async Task<IActionResult> List()
        {
            var appDbContext = _context.Feriado.Include(f => f.idcidadeNavigation);
            return View(await appDbContext.ToListAsync());
        }       


        // GET: feriados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feriado = await _context.Feriado
                .Include(f => f.idcidadeNavigation)
                .FirstOrDefaultAsync(m => m.idferiado == id);
            if (feriado == null)
            {
                return NotFound();
            }

            return View(feriado);
        }

        // GET: feriados/Create
        public IActionResult Create()
        {
            ViewData["idcidade"] = new SelectList(_context.Cidade, "idcidade", "nome");
            return View();
        }

        // POST: feriados/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("idferiado,idcidade,dia")] feriado feriado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(feriado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            ViewData["idcidade"] = new SelectList(_context.Cidade, "idcidade", "nome", feriado.idcidade);
            return View(feriado);
        }

        // GET: feriados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feriado = await _context.Feriado.FindAsync(id);
            if (feriado == null)
            {
                return NotFound();
            }
            ViewData["idcidade"] = new SelectList(_context.Cidade, "idcidade", "nome", feriado.idcidade);
            return View(feriado);
        }

        // POST: feriados/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idferiado,idcidade,dia")] feriado feriado)
        {
            if (id != feriado.idferiado)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feriado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!feriadoExists(feriado.idferiado))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(List));
            }
            ViewData["idcidade"] = new SelectList(_context.Cidade, "idcidade", "nome", feriado.idcidade);
            return View(feriado);
        }

        // GET: feriados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feriado = await _context.Feriado
                .Include(f => f.idcidadeNavigation)
                .FirstOrDefaultAsync(m => m.idferiado == id);
            if (feriado == null)
            {
                return NotFound();
            }

            return View(feriado);
        }

        // POST: feriados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feriado = await _context.Feriado.FindAsync(id);
            _context.Feriado.Remove(feriado);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        private bool feriadoExists(int id)
        {
            return _context.Feriado.Any(e => e.idferiado == id);
        }


        // POST: feriados/Delete/5
        [HttpPost, ActionName("SaveFeriado")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFeriado(int idcidade, DateTime dia) {
            var feriado = await _context.Feriado.Where(p => p.idcidade == idcidade && p.dia == dia).FirstOrDefaultAsync();
            if (feriado != null)
            {
                _context.Feriado.Remove(feriado);
            }else
            {
                _context.Add(new feriado() { idcidade = idcidade, dia = dia });
            }
            await _context.SaveChangesAsync();
            return Json("Ok");
        }

        [HttpGet, ActionName("GetFeriados")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> GetFeriados(int idcidade)
        {
            var feriados = await _context.Feriado.Where(p => p.idcidade == idcidade ).Select(p=>p.dia).ToListAsync();
            var feriadosbr = await _context.Feriado.Where(p => p.idcidade == 0).Select(p => p.dia).ToListAsync();
            if (idcidade == 0) feriadosbr.Clear();
            return Json( new { feriados=feriados, feriadosbr= feriadosbr } );
        }

    }
}
