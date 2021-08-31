using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Identity;
using modelo_core_mvc.Models;
using Microsoft.AspNetCore.Authorization;
using modelo_core_mvc.HttpClients;
using System.Threading.Tasks;

namespace modelo_core_mvc.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private IConfiguration Configuration;
        private readonly ProjetosApiClient _api;

        public string NomeUsuario { get; private set; }

        public HomeController(IConfiguration configuration, ProjetosApiClient api, Usuario usuario)
        {
            Configuration = configuration;
            _api = api;
            if (usuario.Nome != null)
            { NomeUsuario = usuario.Nome; }
            else
                NomeUsuario = "visitante";
        }

        public IActionResult Index()
        {
            ViewData["Usuario"] = NomeUsuario;
            return View();
        }

        public IActionResult Privacidade()
        {
            ViewData["Usuario"] = NomeUsuario;
            ViewData["Title"] = "Privacidade";
            return View();
        }

        public IActionResult TesteIdentity()
        {
            ViewData["Usuario"] = NomeUsuario;
            ViewData["Title"] = "Teste do Identity";
            return View();
        }

        public async Task<ActionResult> Sobre()
        {
            ViewData["Usuario"] = NomeUsuario;
            ViewData["Title"] = "Sobre";
            ViewData["Message"] = "Sobre essa aplicação";
            ViewData["status"] = await _api.GetStatusAsync();
            ViewData["conexao"] = await _api.GetConexaoAsync();

            return View();
        }

        public IActionResult Contato()
        {
            ViewData["Usuario"] = NomeUsuario;
            ViewData["Title"] = "Contato";
            ViewData["Message"] = "Fale conosco";

            return View();
        }

        public IActionResult Sair()
        {
            ViewData["Usuario"] = NomeUsuario;
            ViewData["Title"] = "Sair";
            ViewData["Message"] = "Encerrar a sessão";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewData["Usuario"] = NomeUsuario;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
