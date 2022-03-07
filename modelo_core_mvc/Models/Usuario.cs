using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace modelo_core_mvc.Models
{
    public class Usuario
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IHttpClientFactory _clientFactory;
        private IConfiguration _configuration;

        public string Token { get; set; }
        private XmlDocument TokenSAML { get; set; }
        [Display(Name = "email")]
        public string Login { get; set; }
        public string Nome { get; set; }

        //Não consigo acessar a propriedade Nome na view _loginParcial, se não for estática, e não consigo acessar em outros lugares se for estática
        //Não soube resolver, se não duplicar a propriedade
        [Display(Name = "Doc. Identificação")]
        public string DocumentoIdentificacao { get; set; }
        [Display(Name = "Nascimento")]
        public string DataNascimento { get; set; }

        public Usuario(IHttpContextAccessor acessor, IHttpClientFactory clientFactory, ITokenAcquisition tokenAcquisition, IConfiguration configuration)

        {
            _tokenAcquisition = tokenAcquisition;
            _clientFactory = clientFactory;
            _configuration = configuration;

            TokenSAML = new XmlDocument();
            if (acessor.HttpContext.User.Identities.First().BootstrapContext != null)
            {
                Token = acessor.HttpContext.User.Identities.First().BootstrapContext.ToString();
                //Se BootstrapContext tem conteúdo, estamos no contexto da autenticação na aplicação
                TokenSAML.LoadXml(Token);
                Token = Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(Token));
            }
            else
            {
                //Se não, estamos tratando da autorização que chegou na requisição à api
                if (acessor.HttpContext.Request.Headers["Authorization"].Count > 0)
                {
                    try { TokenSAML.LoadXml(UTF8Encoding.UTF8.GetString(Convert.FromBase64String(acessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1]))); }
                    catch { }
                }
            }

            var claims = acessor.HttpContext.User.Claims.ToList();
            foreach (Claim claim in claims)
            {
                var valor = claim.Value;
                var campo = claim.Type;
                campo = campo.Split("/")[campo.Split("/").Count() - 1];
                if (!string.IsNullOrEmpty(campo))
                {
                    if ((campo == "upn") | (campo == "preferred_username"))
                        Login = valor;
                    else
                    if (campo == "name")
                    {
                        Nome = valor.Split(":")[0];
                        if (valor.Split(":").Count() > 1)
                        {
                            DocumentoIdentificacao = valor.Split(":")[1];
                        }
                    }
                    else
                    if (campo == "CNPJ")
                        DocumentoIdentificacao = valor;
                    else
                    if (campo == "CPF")
                        DocumentoIdentificacao = valor;
                    else
                    if (campo == "dateofbirth")
                        DataNascimento = valor;
                }
            }
        }

        public async Task<JArray> GetApiDataAsync()
        {
            try
            {
                var client = _clientFactory.CreateClient();

                var scope = _configuration["CallApi:ScopeForAccessToken"];
                var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { scope });

                client.BaseAddress = new Uri(_configuration["CallApi:ApiBaseAddress"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync("weatherforecast");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var data = JArray.Parse(responseContent);

                    return data;
                }

                throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }
    }
}
