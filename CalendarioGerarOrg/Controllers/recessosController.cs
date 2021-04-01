using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CalendarioGerarOrg.Models;
using Microsoft.Extensions.Configuration;

namespace CalendarioGerarOrg.Controllers
{
    public class recessosController : Controller
    {
        private readonly AppDbContext _context;

        public recessosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: recessos
        public async Task<IActionResult> Index([FromServices] IConfiguration config)
        {
            var anos = await _context.Feriado.Select(p => p.dia.Year).Distinct().OrderBy(p => p).ToListAsync();
            anos.Add(anos.Max() + 1);
            anos.Add(anos.Max() + 1);
            var cidades = await _context.Cidade.OrderBy(p => p.nome).ToListAsync();

            if(!config.GetValue<bool>("ShowGeral")) { 
                var geral = cidades.Single(p => p.idcidade == 0);
                cidades.Remove(geral);
                //geral.nome = "";
            }
            cidades.Insert(0, new cidade() { idcidade=-1,nome="" });
            ViewData["cidade"] = new SelectList(cidades, "idcidade", "nome", -1);
            ViewData["ano"] = new SelectList(anos, DateTime.Now.Year);
            

              var tipos=  Enum.GetValues(typeof(recessoTipo))
              .Cast<recessoTipo>()
            //.Select(v => new {id= (int)v, nome=v.ToString().Replace('_',' ') })
            .Select(v => new { id = (int)v, nome = v.ToString().Replace("Recesso_Gerar_", "") })
            .ToList().OrderBy(v=>v.nome);
            ViewData["tipo"] = new SelectList(tipos, "id", "nome", 0);
            return View();
        }
        public async Task<IActionResult> List()
        {
            var appDbContext = _context.Recesso.Include(r => r.idcidadeNavigation);
            return View(await appDbContext.ToListAsync());
        }
        // GET: recessos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recesso = await _context.Recesso
                .Include(r => r.idcidadeNavigation)
                .FirstOrDefaultAsync(m => m.idrecesso == id);
            if (recesso == null)
            {
                return NotFound();
            }

            return View(recesso);
        }

        // GET: recessos/Create
        public IActionResult Create()
        {
            ViewData["idcidade"] = new SelectList(_context.Cidade, "idcidade", "idcidade");
            return View();
        }

        // POST: recessos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("idrecesso,idcidade,dia,tipo")] recesso recesso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recesso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["idcidade"] = new SelectList(_context.Cidade, "idcidade", "idcidade", recesso.idcidade);
            return View(recesso);
        }

        // GET: recessos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recesso = await _context.Recesso.FindAsync(id);
            if (recesso == null)
            {
                return NotFound();
            }
            ViewData["idcidade"] = new SelectList(_context.Cidade, "idcidade", "idcidade", recesso.idcidade);
            return View(recesso);
        }

        // POST: recessos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idrecesso,idcidade,dia,tipo")] recesso recesso)
        {
            if (id != recesso.idrecesso)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recesso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!recessoExists(recesso.idrecesso))
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
            ViewData["idcidade"] = new SelectList(_context.Cidade, "idcidade", "idcidade", recesso.idcidade);
            return View(recesso);
        }

        // GET: recessos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recesso = await _context.Recesso
                .Include(r => r.idcidadeNavigation)
                .FirstOrDefaultAsync(m => m.idrecesso == id);
            if (recesso == null)
            {
                return NotFound();
            }

            return View(recesso);
        }

        // POST: recessos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recesso = await _context.Recesso.FindAsync(id);
            _context.Recesso.Remove(recesso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool recessoExists(int id)
        {
            return _context.Recesso.Any(e => e.idrecesso == id);
        }

        [HttpGet, ActionName("GetRecessos")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> GetRecessos(int idcidade, int tipo)
        {
            var lista = await _context.Recesso.Where(p => p.idcidade == idcidade && p.tipo==tipo).Select(p => p.dia).ToListAsync(); //pega os recessos
            var feriadosbr = await _context.Feriado.Where(p => p.idcidade == 0 || p.idcidade== idcidade).Select(p => p.dia).ToListAsync(); //pega os feriados br e da cidad selecionada
            
            var listabr = await _context.Recesso.Where(p => p.idcidade == 0 && p.tipo == tipo).Select(p => p.dia).ToListAsync();// pega os recessos br
            if (idcidade == 0) listabr.Clear();
            listabr=listabr.Union(feriadosbr).ToList();
            return Json(new { feriados = lista, feriadosbr = listabr.Distinct() });
        }
        // POST: feriados/Delete/5
        [HttpPost, ActionName("SaveFeriado")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFeriado(int idcidade, DateTime dia, int tipo)
        {
            var recesso = await _context.Recesso.Where(p => p.idcidade == idcidade && p.dia == dia&& p.tipo==tipo).FirstOrDefaultAsync();
            if (recesso != null)
            {
                _context.Recesso.Remove(recesso);
            }
            else
            {
                _context.Add(new recesso() { idcidade = idcidade, dia = dia , tipo=tipo});
            }
            await _context.SaveChangesAsync();
            return Json("Ok");
        }
    }
}
