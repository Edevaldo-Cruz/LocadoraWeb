using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LocadoraWeb.Context;
using LocadoraWeb.Model;

namespace LocadoraWeb.Controllers
{
    public class LocacaoController : Controller
    {
        private readonly AppDbContext _context;

        public LocacaoController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Locacoes.Include(l => l.Cliente).Include(l => l.Filme);
            return View(await appDbContext.ToListAsync());
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Locacoes == null)
            {
                return NotFound();
            }

            var locacao = await _context.Locacoes
                .Include(l => l.Cliente)
                .Include(l => l.Filme)
                .FirstOrDefaultAsync(m => m.LocacaoId == id);
            if (locacao == null)
            {
                return NotFound();
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nome", locacao.ClienteId);
            ViewData["FilmeId"] = new SelectList(_context.Filmes, "FilmeId", "Titulo", locacao.FilmeId);

            ViewBag.dataLocacao = locacao.DataLocacao.ToString("dd/MM/yyyy");
            ViewBag.dataEntrega = locacao.DataDevolucao.ToString("dd/MM/yyyy");
            return View(locacao);
        }

        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nome");
            ViewData["FilmeId"] = new SelectList(_context.Filmes, "FilmeId", "Titulo");
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AlugarFilme(int clienteId, int filmeId)
        {
            var filmeEscolhido = _context.Filmes.Find(filmeId);
            var cliente = _context.Clientes.Find(clienteId);

            DateTime dataAtual = DateTime.Now;
            Locacao novaLocacao = new Locacao();
            novaLocacao.ClienteId = clienteId;
            novaLocacao.FilmeId = filmeId;
            novaLocacao.DataLocacao = dataAtual;

            if (filmeEscolhido.Lancamento == 1)
            {
                novaLocacao.DataDevolucao = dataAtual.AddDays(2);
            }
            else
            {
                novaLocacao.DataDevolucao = dataAtual.AddDays(3);
            }

            novaLocacao.Cliente = cliente;
            novaLocacao.Filme = filmeEscolhido;

            if (ModelState.IsValid)
            {
                _context.Add(novaLocacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(); ;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocacaoId,ClienteId,FilmeId,DataLocacao,DataDevolucao,Devolvido")] Locacao locacao)
        {
            var filmeEscolhido = _context.Filmes.Find(locacao.FilmeId);
            DateTime dataAtual = DateTime.Now;


            Locacao novaLocacao = new Locacao();
            novaLocacao.ClienteId = locacao.ClienteId;
            novaLocacao.FilmeId = locacao.FilmeId;
            novaLocacao.DataLocacao = dataAtual;

            if (filmeEscolhido.Lancamento == 1)
            {
                novaLocacao.DataDevolucao = dataAtual.AddDays(2);
            }
            else
            {
                novaLocacao.DataDevolucao = dataAtual.AddDays(3);
            }
            _context.Add(novaLocacao);
            await _context.SaveChangesAsync();

            int id = _context.Locacoes
                .OrderBy(x => x.LocacaoId)
                .Select(x => x.LocacaoId)
                .Last();
            return RedirectToAction("Details", new { id });

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nome", locacao.ClienteId);
            ViewData["FilmeId"] = new SelectList(_context.Filmes, "FilmeId", "FilmeId", locacao.FilmeId);
            return View(locacao);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Locacoes == null)
            {
                return NotFound();
            }

            var locacao = await _context.Locacoes.FindAsync(id);
            if (locacao == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nome", locacao.ClienteId);
            ViewData["FilmeId"] = new SelectList(_context.Filmes, "FilmeId", "Titulo", locacao.FilmeId);

            ViewBag.dataLocacao = locacao.DataLocacao.ToString("dd/MM/yyyy");
            ViewBag.dataEntrega = locacao.DataDevolucao.ToString("dd/MM/yyyy");
            return View(locacao);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("LocacaoId,ClienteId,FilmeId,DataLocacao,DataDevolucao,Devolvido")] Locacao locacao)
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nome", locacao.ClienteId);
            ViewData["FilmeId"] = new SelectList(_context.Filmes, "FilmeId", "Titulo", locacao.FilmeId);

            ViewBag.dataLocacao = locacao.DataLocacao.ToString("dd/MM/yyyy");
            ViewBag.dataEntrega = locacao.DataDevolucao.ToString("dd/MM/yyyy");


            var locacaoSelecionada = _context.Locacoes.Find(id);
            locacaoSelecionada.DataLocacao = locacao.DataLocacao;
            locacaoSelecionada.DataDevolucao = locacao.DataDevolucao;
            locacaoSelecionada.Devolvido = locacao.Devolvido;

            _context.Locacoes.Update(locacaoSelecionada);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Locacoes == null)
            {
                return NotFound();
            }

            var locacao = await _context.Locacoes
                .Include(l => l.Cliente)
                .Include(l => l.Filme)
                .FirstOrDefaultAsync(m => m.LocacaoId == id);
            if (locacao == null)
            {
                return NotFound();
            }

            return View(locacao);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Locacoes == null)
            {
                return Problem("Entity set 'AppDbContext.Locacoes'  is null.");
            }
            var locacao = await _context.Locacoes.FindAsync(id);
            if (locacao != null)
            {
                _context.Locacoes.Remove(locacao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult<Locacao>> DevolverFilme(int id)
        {
            var locacao = _context.Locacoes.Find(id);
            locacao.Devolvido = true;

            _context.Locacoes.Update(locacao);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool LocacaoExists(int id)
        {
            return _context.Locacoes.Any(e => e.LocacaoId == id);
        }
    }
}
