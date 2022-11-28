using LocadoraWeb.Context;
using LocadoraWeb.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraWeb.Controllers
{
    public class RelatorioController : Controller
    {
        private readonly AppDbContext _context;

        public RelatorioController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult RetornaLocacacoesAtrasadas()
        {
            var locacoes = _context.Locacoes
                .Where(l => l.Devolvido == false && l.DataDevolucao < DateTime.Now)
                .Include(c => c.Cliente)
                .Include(f => f.Filme)
                .ToList();

            List<Locacao> model = new List<Locacao>();
            model.AddRange(locacoes);
            return View(model);
        }

        public IActionResult RetornaFilmeNuncaAlugados()
        {
            var filmes = _context.Filmes.Select(x => x.FilmeId).ToList();
            var filmesAlugados = _context.Locacoes.Select(x => x.FilmeId).ToList();
            var filmesNuncaAlugados = filmes.Except(filmesAlugados).ToList();

            List<Filme> model = new List<Filme>();
            foreach (var filme in filmesNuncaAlugados)
            {
                var filmeAtual = _context.Filmes.Find(filme);
                model.Add(filmeAtual);
            }

            return View(model);
        }

        public IActionResult RetornaFilmesMaisAlugados()
        {
            var dataAtual = DateTime.Now;
            var ultimoAno = dataAtual.AddYears(-1);

            var filmesAlugados = _context.Locacoes
                                    .Where(f => f.DataLocacao < dataAtual
                                        && f.DataLocacao > ultimoAno)
                                    .ToList();

            var lista = filmesAlugados
                        .GroupBy(x => x.FilmeId)
                        .Where(g => g.Count() >= 1)
                        .OrderByDescending(g => g.Count())
                        .Select(x => x.Key)
                        .ToList().Take(5);


            List<Filme> model = new List<Filme>();
            foreach (var filme in lista)
            {
                var filmeAtual = _context.Filmes.Find(filme);
                model.Add(filmeAtual);
            }
            return View(model);
        }

        public IActionResult RetornaFilmesMenosAlugadosDaSemana()
        {
            var dataAtual = DateTime.Now;
            var ultimaSemana = dataAtual.AddDays(-7);

            var filmesAlugados = _context.Locacoes
                                    .Where(f => f.DataLocacao < dataAtual
                                        && f.DataLocacao > ultimaSemana)
                                    .ToList();

            var listaFilmesAlugado = filmesAlugados
                        .GroupBy(x => x.FilmeId)
                        .Where(g => g.Count() >= 1)
                        .OrderByDescending(g => g.Count())
                        .Select(x => x.Key)
                        .Reverse()
                        .ToList();

            var filmes = _context.Filmes.Select(x => x.FilmeId).ToList();
            var todosFilmesAlugados = _context.Locacoes.Select(x => x.FilmeId).ToList();
            var filmesNuncaAlugados = filmes.Except(todosFilmesAlugados).ToList();

            List<Filme> model = new List<Filme>();
            int contador = 3;
            foreach (var filme in filmesNuncaAlugados)
            {
                if (contador != 0)
                {
                    var filmeAtual = _context.Filmes.Find(filme);
                    model.Add(filmeAtual);
                    contador--;
                }
                else
                {
                    break;
                }
            }
            if (contador != 0)
            {
                foreach (var filme in listaFilmesAlugado)
                {
                    if (contador != 0)
                    {
                        var filmeAtual = _context.Filmes.Find(filme);
                        model.Add(filmeAtual);
                        contador--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return View(model);
        }

        public IActionResult RetornaSegundoCliente()
        {
            var clientes = _context.Locacoes.ToList();
            var lista = clientes
                        .GroupBy(x => x.ClienteId)
                        .Where(g => g.Count() > 1)
                        .Select(x => x.Key)
                        .ToList().Take(2);

            List<Cliente> model = new List<Cliente>();

            var segundoCliente = _context.Clientes.Find(lista.Last());
            model.Add(segundoCliente);

            return View(model);
        }
    }
}