using modelo_core_mvc.projetos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using modelo_core_mvc.HttpClients;
using Identity;
using Microsoft.AspNetCore.Authorization;

namespace modelo_core_mvc.Controllers
{
    [Authorize]
    public class ProjetosController : Controller
    {
        private readonly ProjetosApiClient _api;
        public string NomeUsuario { get; private set; }

        public ProjetosController(ProjetosApiClient api, Usuario usuario)
        {
            _api = api;
            NomeUsuario = usuario.Nome;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ViewData["Usuario"] = NomeUsuario;
            ViewData["Title"] = "Projetos";
            ViewData["Message"] = "Projetos do DTI";

            return View(await _api.GetProjetosAsync());
        }

        [HttpGet]
        public async Task<ActionResult> Detalhes(long cd_projeto)
        {
            ViewData["Usuario"] = NomeUsuario;
            ViewData["Title"] = "Projeto";
            ViewData["Message"] = "";
            return View(await _api.GetProjetoAsync(cd_projeto));
        }

        [HttpGet]
        public ActionResult Adicionar()
        {
            ViewData["Usuario"] = NomeUsuario;
            ViewData["Title"] = "Novo Projeto";
            ViewData["Message"] = "Incluir novo projeto";
            return View(new Projetos());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Adicionar(Projetos model)
        {
            if (ModelState.IsValid)
            {
                await _api.PostProjetoAsync(model);
                return RedirectToAction("Index");
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<ActionResult> Alterar(long cd_projeto)
        {
            ViewData["Usuario"] = NomeUsuario;
            ViewData["Title"] = "Editar Projeto";
            ViewData["Message"] = "Editar informações do projeto";
            var model = await _api.GetProjetoAsync(cd_projeto);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Alterar(Projetos model)
        {
            if (ModelState.IsValid)
            {
                await _api.PutProjetoAsync(model);
                return RedirectToAction("Index");
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<ActionResult> Excluir(long cd_projeto)
        {
            ViewData["Usuario"] = NomeUsuario;
            ViewData["Title"] = "Excluir Projeto";
            ViewData["Message"] = "Exclusão do projeto";
            var model = await _api.GetProjetoAsync(cd_projeto);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Excluir(Projetos model)
        {
            if (ModelState.IsValid)
            {
                await _api.DeleteProjetoAsync(model.cd_projeto);
                return RedirectToAction("Index");
            }
            return BadRequest();
        }
    }
}
