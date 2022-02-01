using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using modelo_core_mvc.Models;
using modelo_core_mvc.HttpClients;
using System.Threading.Tasks;

namespace modelo_core_mvc.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration Configuration;
        private readonly ProjetosApiClient _api;


        //Insercao de teste de vulnerabilidade
        private readonly string[] whiteList = { "https://ads.intra.fazenda.sp.gov.br/tfs" };
        public IActionResult RedirectMe(string url)
        {
            return Redirect(url);
        }
        //Fim do teste

        public HomeController(IConfiguration configuration, ProjetosApiClient api, Usuario usuario)
        {
            Configuration = configuration;
            _api = api;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacidade()
        {
            ViewData["Title"] = "Privacidade";
            return View();
        }

        public IActionResult TesteIdentity()
        {
            ViewData["Title"] = "Teste do Identity";
            return View();
        }

        public async Task<ActionResult> Sobre()
        {
            ViewData["Title"] = "Sobre";
            ViewData["Message"] = "Sobre essa aplicação";
            ViewData["status"] = await _api.GetStatusAsync();
            ViewData["conexao"] = await _api.GetConexaoAsync();
            ViewData["EnderecoAPI"] = Configuration["apiendereco:projetos"];

            return View();
        }

        public IActionResult Contato()
        {
            ViewData["Title"] = "Contato";
            ViewData["Message"] = "Fale conosco";

            return View();
        }

        public IActionResult Sair()
        {
            ViewData["Title"] = "Sair";
            ViewData["Message"] = "Encerrar a sessão";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
