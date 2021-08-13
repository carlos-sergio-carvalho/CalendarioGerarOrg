using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarioGerarOrg.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CalendarioGerarOrg.Controllers
{
    public class calendarioController : Controller
    {

        private readonly AppDbContext _context;

        public calendarioController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //var cidades = await _context.Cidade.OrderBy(p => p.nome).ToListAsync();
            /**/
            var cidades = await _context.Subsede.OrderBy(p => p.nome).Select(p => new cidade()
            { estado = p.idcidadeNavigation.estado,
             idcidade= p.idcidadeNavigation.idcidade, 
             nome= p.nome +" (" +p.idcidadeNavigation.nome +")"
            }).ToListAsync(); 
            
            /*
            var cidades = await _context.Cidade
                .OrderBy(p => p.nome)
                .Select(p => new cidade()
                {
                    estado = p.estado,
                    idcidade = p.idcidade,
                    nome = p.nome + " - " + p.estado
                }
                ).ToListAsync();
            */
            //var t = from s in _context.Subsede select new cidade { };
            //var geral = cidades.Single(p => p.idcidade == 0);
            //cidades.Remove(geral);
            //cidades.Insert(0, geral);
            ViewData["cidade"] = new SelectList(cidades, "idcidade", "nome");

            calendario cal = new calendario();
            cal.datainicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day); 
            
            //cal.datafinal = cal.datainicial.AddMonths(1);
            cal.cargahoraria = 0;
            cal.semanateoria = 1;
            cal.idcidade = cidades.First().idcidade;

            return View(UpdateCalendario(cal));
        }

        [HttpGet, ActionName("GetFeriados")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> GetFeriados(int idcidade)
        {
            var feriados = await _context.Feriado.Where(p => p.idcidade == idcidade || p.idcidade==0).Select(p => p.dia).Distinct().ToListAsync();
            return Json(feriados);
        }


        [HttpPost, ActionName("GetCalendario")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> GetCalendario(calendario cal)
        {
            return await Task.FromResult(Json(UpdateCalendario(cal))); 

            //cal.idcidadeNavigation = await _context.Cidade.Include(p=>p.feriados).SingleAsync(p => p.idcidade == cal.idcidade);
            //pegar somente os valores do intervalo ?
            /*cal.feriados = await _context.Feriado.Where(p => p.idcidade == cal.idcidade || p.idcidade == 0).Select(p => p.dia).Distinct().ToListAsync();
            cal.teoricas = new List<DateTime>();
            cal.praticas = new List<DateTime>();
            foreach (DateTime day in EachDay(cal.datainicial, cal.datafinal))
            {
                //check , sabado domingo,feriado 
                if (day.DayOfWeek!= DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday && !cal.feriados.Any(p => p == day) )
                {
                    //check se o dia é da teorica
                    if ((int)day.DayOfWeek == cal.semanateoria)
                    {
                        cal.teoricas.Add(day);
                    }
                    else
                    {
                        cal.praticas.Add(day);
                    }
                }
            }
            cal.extras?.ToList().ForEach(p => {
                foreach (DateTime day in EachDay(p.datainicial, p.datafinal))
                {

                    //check , sabado domingo,feriado 
                    if (day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday && !cal.feriados.Any(pp => pp == day))
                    {
                        cal.teoricas.Add(day);
                    }

                }
            });
            return Json(cal);*/
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        private calendario UpdateCalendario(calendario cal)
        {
                cal.log = new List<dynamic>();
            
            
            /*
             0- 20/17 - 1280 - 400/880
             1- 20/24 - 1840 - 552/1288
             2- 30/11 - 1280 - 400/880
             3- 30/15 - 1840 - 552/1288
             */
            int horaspraticas = 0;
            int horasteoricas = 0;
            //int horaspraticastotal = 0;
            //int horasteoricastotal = 0;
            int horadia = (cal.cargahoraria== 0 || cal.cargahoraria==1) ?4:6;
            switch (cal.cargahoraria)
            {
                case 0: case 2:
                    cal.cargateorica = 400;
                    cal.cargapratica = 880;
                    // horaspraticastotal = 880;
                    //horasteoricastotal = 400;
                    break;
                case 1: case 3:
                    //horaspraticastotal = 1288;
                    //horasteoricastotal = 552;
                    cal.cargateorica = 552;
                    cal.cargapratica = 1288;
                    break;
            }
            
            cal.feriados = _context.Feriado.Where(p => p.idcidade == cal.idcidade || p.idcidade == 0).Select(p => p.dia).Distinct().ToList();
            cal.recessos = new List<recesso>();
            cal.teoricas = new List<DateTime>();
            cal.praticas = new List<DateTime>();
            cal.recessosgerar = new List<DateTime>();

            cal.feriasdias = new List<DateTime>();
            cal.ferias?.ToList().ForEach(p => cal.feriasdias.AddRange(EachDay(p.datainicial, p.datafinal)));

            cal.suspensaodias = new List<DateTime>();
            cal.suspensao?.ToList().ForEach(p => cal.suspensaodias.AddRange(EachDay(p.datainicial, p.datafinal)));
            
            cal.reducaodias = new List<DateTime>();
            cal.reducao?.Where(p=>p.datainicial.Year>2000).ToList().ForEach(p => cal.reducaodias.AddRange(EachDay(p.datainicial, p.datafinal)));

            var extrasdias = new List<DateTime>();
            cal.extras?.ToList().ForEach(p => extrasdias.AddRange(EachDay(p.datainicial, p.datafinal)));
            var iniciaisdias = new List<DateTime>();
            cal.iniciais?.ToList().ForEach(p => iniciaisdias.AddRange(EachDay(p.datainicial, p.datafinal)));

            var manuaisteorica = new List<DateTime>();
            cal.manuaisteorica?.ToList().ForEach(p => manuaisteorica.AddRange(EachDay(p.datainicial, p.datafinal)));

            var manuaispratica = new List<DateTime>();
            cal.manuaispratica?.ToList().ForEach(p => manuaispratica.AddRange(EachDay(p.datainicial, p.datafinal)));

            var estabilidadepraticas = new List<DateTime>();
            cal.estabilidadepraticas?.ToList().ForEach(p => estabilidadepraticas.AddRange(EachDay(p.datainicial, p.datafinal)));

            var estabilidadeteoricas = new List<DateTime>();
            cal.estabilidadeteoricas?.ToList().ForEach(p => estabilidadeteoricas.AddRange(EachDay(p.datainicial, p.datafinal)));

            var recessos =      _context.Recesso.Where(p => p.tipo != (int)recessoTipo.Recesso_Gerar_Regular && ( p.idcidade == cal.idcidade || p.idcidade == 0)).ToList();
            var recessosGerar = _context.Recesso.Where(p => p.tipo == (int)recessoTipo.Recesso_Gerar_Regular && (p.idcidade == cal.idcidade || p.idcidade == 0)).ToList();

            var recessosGerarIniciais = _context.Recesso.Where(p => p.tipo == (int)recessoTipo.Recesso_Gerar_Inicial && (p.idcidade == cal.idcidade || p.idcidade == 0)).ToList();
            var recessosGerarExtra = _context.Recesso.Where(p => p.tipo == (int)recessoTipo.Recesso_Gerar_Extra && (p.idcidade == cal.idcidade || p.idcidade == 0)).ToList();

            

            var day = cal.datainicial;
            /*dia fixo*/
            if (cal.datafixa && cal.reducaoper>0)
            {//
               // cal.cargateorica = cal.cargateorica - (cal.reducaodias.Count * horadia);
            }
            /**/
            //cal.feriasdias = feriasdias;
            while (true) // trocar por for quando estiver pronto
            {

                dynamic logday = new System.Dynamic.ExpandoObject();
                cal.log.Add(logday);
                logday.day = day.ToString("dd/MM/yyyy");


                //checar se é suspensao !!!
                if (cal.suspensaodias.Any(p => p == day))
                { logday.tipo = "suspensao"; }
                else
                
                //checar se é ferias !!!
                if (cal.feriasdias.Any(p => p == day))
                { logday.tipo = "ferias"; }
                else
                //check , sabado domingo,feriado 
                if (day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday && !cal.feriados.Any(p => p == day))//
                    {

                    
                            // dia 01/03/19  raphael mudou ordem para  inicial, extra, regular para o gerar
                        if (recessosGerarIniciais.Any(p => p.dia == day) && iniciaisdias.Any(p => p == day))//recessosGerarIniciais 
                        {
                            if (horaspraticas < cal.cargapratica)
                            {
                                cal.recessosgerar.Add(day);
                                horaspraticas += horadia; logday.tipo = "recessosGerarIniciais";
                            }
                        }
                        else
                        if (recessosGerarExtra.Any(p => p.dia == day) && extrasdias.Any(p => p == day))//recessosGerarExtra
                        {
                            if (horaspraticas < cal.cargapratica)
                            {
                                cal.recessosgerar.Add(day);
                                horaspraticas += horadia; logday.tipo = "recessosGerarExtra";
                        }
                        }
                        else
                        if (recessosGerar.Any(p => p.dia == day)  && !iniciaisdias.Any(p => p == day) && !extrasdias.Any(p => p == day))/*recessos gerar regular em dias não extra e iniciais */
                        {
                            if (horaspraticas < cal.cargapratica)
                            {
                                cal.recessosgerar.Add(day);
                                horaspraticas += horadia; logday.tipo = "recessos gerar regular em dias não extra e iniciais";
                            }else { cal.recessosgerar.Add(day); /* ficou errado isso mas o raphael pediu assim */ }
                        }
                        else
                        if(false)// (iniciaisdias.Any(p => p == day) && recessos.Any(p => p.dia == day && p.tipo == (int)recessoTipo.Regular) || /*recessos regulares em Iniciais*/
                            //extrasdias.Any(p => p == day) && recessos.Any(p => p.dia == day && p.tipo == (int)recessoTipo.Regular)  /*recessos regulares em extras*/
                            //)
                        {
                            if (horasteoricas < cal.cargateorica)
                            {
                                cal.teoricas.Add(day);
                                horasteoricas += horadia;
                            }
                        }
                        else
                        if (iniciaisdias.Any(p => p == day) && recessos.Any(p => p.dia == day && p.tipo == (int)recessoTipo.Inicial)/*recessos Iniciais*/)
                        { cal.recessos.Add(new recesso() { dia = day, tipo = (int)recessoTipo.Inicial }); logday.tipo = "recessos Iniciais"; }else
                        if(extrasdias.Any(p => p == day) && recessos.Any(p => p.dia == day && p.tipo == (int)recessoTipo.Extra))  /*recessos extra*/
                        { cal.recessos.Add(new recesso() { dia = day, tipo = (int)recessoTipo.Extra }); logday.tipo = "recessos extra"; }
                    else
                        if(recessos.Any(p => p.dia == day && p.tipo == (int)recessoTipo.Regular) && !iniciaisdias.Any(p => p == day) && !extrasdias.Any(p => p == day))  /*recessos normais em dias não extra e iniciais */
                        { cal.recessos.Add( new recesso() { dia= day, tipo= (int)recessoTipo.Regular }); logday.tipo = "recessos normais em dias não extra e iniciais"; }
                        else
                        //checar se é reducao !!!
                    if (cal.reducaoper > 0 && cal.reducaodias.Any(p => p == day) && !manuaisteorica.Any(p => p == day) && !manuaispratica.Any(p => p == day))
                    { logday.tipo = "reducao"; }
                    else
                    if(estabilidadepraticas.Any(p=> p == day)) { cal.praticas.Add(day); } //essas são especiais ew não contam no total
                        else
                        if (estabilidadeteoricas.Any(p => p == day)) { cal.teoricas.Add(day); }
                    else
                        if (((int)day.DayOfWeek == cal.semanateoria || extrasdias.Any(p => p == day) || iniciaisdias.Any(p => p == day) || manuaisteorica.Any(p => p == day)))//check se o dia é da teorica
                        {
                            if (horasteoricas < cal.cargateorica )
                            {
                                cal.teoricas.Add(day);
                                horasteoricas += horadia; logday.tipo = "t"; 
                            if (iniciaisdias.Any(p => p == day)) { cal.cargainicial++; } //&& (int)day.DayOfWeek != cal.semanateoria foi removido a pedido raphael luiz e samuel em reunião em 26/08/2020

                            
                            
                            if(iniciaisdias.Any(p => p == day))
                            {
                                logday.tipo += " dia iniciais";
                            }else if(extrasdias.Any(p => p == day)) 
                            {
                                logday.tipo += " dia extra";
                            }
                            else if ((int)day.DayOfWeek == cal.semanateoria)
                            {
                                logday.tipo += " dia semana teoria";
                            }
                            else if (manuaisteorica.Any(p => p == day))
                            {
                                logday.tipo += " dia teórico";
                            }

                        }
                        }
                        else
                        {
                            if (horaspraticas < cal.cargapratica)
                            {
                                cal.praticas.Add(day);
                                horaspraticas += horadia;logday.tipo = "n";
                                //if(recessos.Any(p => p.dia == day)) { cal.recessos.Add(day); }
                            }
                        }
                }
                else { logday.tipo = "sabado domingo,feriado";if (cal.reducaodias.Any(p => p == day)) { cal.reducaodias.Remove(day); } }
                day = day.AddDays(1);
                
                logday.horasteoricas = horasteoricas;
                logday.horaspraticas = horaspraticas;
                if (horasteoricas >= cal.cargateorica && horaspraticas>= cal.cargapratica )
                { break; }
                if(cal.datafixa && day==cal.datafinal) { break; }
            }

            if (!cal.datafixa) {
                //cal.datafinal = cal.praticas.Last()> cal.teoricas.Last()?cal.praticas.Last():cal.teoricas.Last();
                var olddate = cal.praticas.Last() > cal.teoricas.Last() ? cal.praticas.Last() : cal.teoricas.Last();
                cal.datafinal = day.AddDays(-1);
                }

            cal.cargainicialhoras = (double)cal.cargainicial * (cal.cargahoraria <= 1 ? 4 : 6);
            cal.cargainicial = Math.Round(cal.cargainicialhoras / cal.cargateorica * 100, 3);

            //estabilidadesuspencao
            
            if (cal.suspensao?.Count > 0 && cal.estabilidadesuspencao)
            {
                cal.estabilidadesuspencaodias = new List<calendarioextra>();
                cal.suspensao.ForEach(p=>{
                    var diasdiff = p.datafinal- p.datainicial;
                    var totaldias = diasdiff.Days * 2;
                    var datatfinales= p.datainicial.AddDays(totaldias);
                    if (cal.datafinal < datatfinales)
                    {
                        cal.estabilidadesuspencaodias.Add(new calendarioextra() { datainicial = p.datainicial, datafinal = datatfinales });
                        cal.datafinal = datatfinales;
                    }
                });
            }


            //subtrair dias de ferias !!!
            //cal.mesestotal = Convert.ToInt32(Math.Floor((cal.datafinal.Subtract(cal.datainicial).Days- cal.feriasdias.Distinct().Count()) / (365.25 / 12)));
            //- cal.feriasdias.Distinct().Count()
            cal.mesestotal = Math.Round((cal.datafinal.Subtract(cal.datainicial).Days ) / (365.25 / 12),2);
            //cal.cargainicial = iniciaisdias.Count;
            //cal.mesestotal =12 * (cal.datafinal.Year - cal.datainicial.Year) + (cal.datafinal.Month - cal.datainicial.Month);
            //cal.mesestotal= cal.praticas + cal.teoricas
            return cal;
        }


        private calendario _UpdateCalendario(calendario cal)
        {
            cal.log = new List<dynamic>();


            /*
             0- 20/17 - 1280 - 400/880
             1- 20/24 - 1840 - 552/1288
             2- 30/11 - 1280 - 400/880
             3- 30/15 - 1840 - 552/1288
             */
            int horaspraticas = 0;
            int horasteoricas = 0;
            //int horaspraticastotal = 0;
            //int horasteoricastotal = 0;
            int horadia = (cal.cargahoraria == 0 || cal.cargahoraria == 1) ? 4 : 6;
            switch (cal.cargahoraria)
            {
                case 0:
                case 2:
                    cal.cargateorica = 400;
                    cal.cargapratica = 880;
                    // horaspraticastotal = 880;
                    //horasteoricastotal = 400;
                    break;
                case 1:
                case 3:
                    //horaspraticastotal = 1288;
                    //horasteoricastotal = 552;
                    cal.cargateorica = 552;
                    cal.cargapratica = 1288;
                    break;
            }

            cal.feriados = _context.Feriado.Where(p => p.idcidade == cal.idcidade || p.idcidade == 0).Select(p => p.dia).Distinct().ToList();
            cal.recessos = new List<recesso>();
            cal.teoricas = new List<DateTime>();
            cal.praticas = new List<DateTime>();
            cal.recessosgerar = new List<DateTime>();

            cal.feriasdias = new List<DateTime>();
            cal.ferias?.ToList().ForEach(p => cal.feriasdias.AddRange(EachDay(p.datainicial, p.datafinal)));

            cal.suspensaodias = new List<DateTime>();
            cal.suspensao?.ToList().ForEach(p => cal.suspensaodias.AddRange(EachDay(p.datainicial, p.datafinal)));

            var extrasdias = new List<DateTime>();
            cal.extras?.ToList().ForEach(p => extrasdias.AddRange(EachDay(p.datainicial, p.datafinal)));
            var iniciaisdias = new List<DateTime>();
            cal.iniciais?.ToList().ForEach(p => iniciaisdias.AddRange(EachDay(p.datainicial, p.datafinal)));

            var recessos = _context.Recesso.Where(p => p.tipo != (int)recessoTipo.Recesso_Gerar_Regular && (p.idcidade == cal.idcidade || p.idcidade == 0)).ToList();
            var recessosGerar = _context.Recesso.Where(p => p.tipo == (int)recessoTipo.Recesso_Gerar_Regular && (p.idcidade == cal.idcidade || p.idcidade == 0)).ToList();

            var recessosGerarIniciais = _context.Recesso.Where(p => p.tipo == (int)recessoTipo.Recesso_Gerar_Inicial && (p.idcidade == cal.idcidade || p.idcidade == 0)).ToList();
            var recessosGerarExtra = _context.Recesso.Where(p => p.tipo == (int)recessoTipo.Recesso_Gerar_Extra && (p.idcidade == cal.idcidade || p.idcidade == 0)).ToList();

            var day = cal.datainicial;
            //cal.feriasdias = feriasdias;
            while (true) // trocar por for quando estiver pronto
            {

                dynamic logday = new System.Dynamic.ExpandoObject();
                cal.log.Add(logday);
                logday.day = day.ToString("dd/MM/yyyy");


                //checar se é suspensao !!!
                if (cal.suspensaodias.Any(p => p == day))
                { logday.tipo = "suspensao"; }
                else
                //checar se é ferias !!!
                if (cal.feriasdias.Any(p => p == day))
                { logday.tipo = "ferias"; }
                else
                //check , sabado domingo,feriado 
                if (day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday && !cal.feriados.Any(p => p == day))//
                {
                    // dia 01/03/19  raphael mudou ordem para  inicial, extra, regular para o gerar
                    if (recessosGerarIniciais.Any(p => p.dia == day) && iniciaisdias.Any(p => p == day))//recessosGerarIniciais 
                    {
                        if (horaspraticas < cal.cargapratica)
                        {
                            cal.recessosgerar.Add(day);
                            horaspraticas += horadia; logday.tipo = "recessosGerarIniciais";
                        }
                    }
                    else
                    if (recessosGerarExtra.Any(p => p.dia == day) && extrasdias.Any(p => p == day))//recessosGerarExtra
                    {
                        if (horaspraticas < cal.cargapratica)
                        {
                            cal.recessosgerar.Add(day);
                            horaspraticas += horadia; logday.tipo = "recessosGerarExtra";
                        }
                    }
                    else
                    if (recessosGerar.Any(p => p.dia == day) && !iniciaisdias.Any(p => p == day) && !extrasdias.Any(p => p == day))/*recessos gerar regular em dias não extra e iniciais */
                    {
                        if (horaspraticas < cal.cargapratica)
                        {
                            cal.recessosgerar.Add(day);
                            horaspraticas += horadia; logday.tipo = "recessos gerar regular em dias não extra e iniciais";
                        }
                        else { cal.recessosgerar.Add(day); /* ficou errado isso mas o raphael pediu assim */ }
                    }
                    else
                    if (false)// (iniciaisdias.Any(p => p == day) && recessos.Any(p => p.dia == day && p.tipo == (int)recessoTipo.Regular) || /*recessos regulares em Iniciais*/
                              //extrasdias.Any(p => p == day) && recessos.Any(p => p.dia == day && p.tipo == (int)recessoTipo.Regular)  /*recessos regulares em extras*/
                              //)
                    {
                        if (horasteoricas < cal.cargateorica)
                        {
                            cal.teoricas.Add(day);
                            horasteoricas += horadia;
                        }
                    }
                    else
                    if (iniciaisdias.Any(p => p == day) && recessos.Any(p => p.dia == day && p.tipo == (int)recessoTipo.Inicial)/*recessos Iniciais*/)
                    { cal.recessos.Add(new recesso() { dia = day, tipo = (int)recessoTipo.Inicial }); logday.tipo = "recessos Iniciais"; }
                    else
                    if (extrasdias.Any(p => p == day) && recessos.Any(p => p.dia == day && p.tipo == (int)recessoTipo.Extra))  /*recessos extra*/
                    { cal.recessos.Add(new recesso() { dia = day, tipo = (int)recessoTipo.Extra }); logday.tipo = "recessos extra"; }
                    else
                    if (recessos.Any(p => p.dia == day && p.tipo == (int)recessoTipo.Regular) && !iniciaisdias.Any(p => p == day) && !extrasdias.Any(p => p == day))  /*recessos normais em dias não extra e iniciais */
                    { cal.recessos.Add(new recesso() { dia = day, tipo = (int)recessoTipo.Regular }); logday.tipo = "recessos normais em dias não extra e iniciais"; }
                    else
                    if (((int)day.DayOfWeek == cal.semanateoria || extrasdias.Any(p => p == day) || iniciaisdias.Any(p => p == day)))//check se o dia é da teorica
                    {
                        if (horasteoricas < cal.cargateorica)
                        {
                            cal.teoricas.Add(day);
                            horasteoricas += horadia; logday.tipo = "t";
                            if (iniciaisdias.Any(p => p == day) && (int)day.DayOfWeek != cal.semanateoria) { cal.cargainicial++; }
                        }
                    }
                    else
                    {
                        if (horaspraticas < cal.cargapratica)
                        {
                            cal.praticas.Add(day);
                            horaspraticas += horadia; logday.tipo = "n";
                            //if(recessos.Any(p => p.dia == day)) { cal.recessos.Add(day); }
                        }
                    }
                }
                else { logday.tipo = "sabado domingo,feriado"; }
                day = day.AddDays(1);

                logday.horasteoricas = horasteoricas;
                logday.horaspraticas = horaspraticas;
                if (horasteoricas >= cal.cargateorica && horaspraticas >= cal.cargapratica)
                { break; }
            }

            cal.datafinal = cal.praticas.Last() > cal.teoricas.Last() ? cal.praticas.Last() : cal.teoricas.Last();
            double ci = (double)cal.cargainicial * (cal.cargahoraria <= 1 ? 4 : 6) / cal.cargateorica * 100;
            cal.cargainicial = Math.Round(ci, 3);

            //subtrair dias de ferias !!!
            //cal.mesestotal = Convert.ToInt32(Math.Floor((cal.datafinal.Subtract(cal.datainicial).Days- cal.feriasdias.Distinct().Count()) / (365.25 / 12)));
            //- cal.feriasdias.Distinct().Count()
            cal.mesestotal = Math.Round((cal.datafinal.Subtract(cal.datainicial).Days) / (365.25 / 12), 2);
            //cal.cargainicial = iniciaisdias.Count;
            //cal.mesestotal =12 * (cal.datafinal.Year - cal.datainicial.Year) + (cal.datafinal.Month - cal.datainicial.Month);
            //cal.mesestotal= cal.praticas + cal.teoricas
            return cal;
        }

        [HttpPost]
        public async Task<IActionResult> SaveCalendario(calendario cal)
        {

            
            if (cal.idcalendario == 0)
            { _context.Add(cal); }
            else
            { _context.Update(cal); }

            await _context.SaveChangesAsync();

            return Ok();
        }
        public async Task<IActionResult> Index2()
        {
            //var cidades = await _context.Cidade.OrderBy(p => p.nome).ToListAsync();
            var cidades = await _context.Subsede.OrderBy(p => p.nome).Select(p => new cidade()
            {
                estado = p.idcidadeNavigation.estado,
                idcidade = p.idcidadeNavigation.idcidade,
                nome = p.nome + " (" + p.idcidadeNavigation.nome + ")"
            }).ToListAsync();
            //var t = from s in _context.Subsede select new cidade { };
            //var geral = cidades.Single(p => p.idcidade == 0);
            //cidades.Remove(geral);
            //cidades.Insert(0, geral);
            ViewData["cidade"] = new SelectList(cidades, "idcidade", "nome");

            calendario cal = new calendario();
            cal.datainicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            //cal.datafinal = cal.datainicial.AddMonths(1);
            cal.cargahoraria = 0;
            cal.semanateoria = 1;
            cal.idcidade = cidades.First().idcidade;

            return View(UpdateCalendario(cal));
        }
    }
}