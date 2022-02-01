using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;

namespace modelo_core_mvc.Models
{
    public class Usuario
    {
        public string Token { get; set; }
        [Display(Name = "email")]
        public string Login { get; set; }
        public string Nome { get; set; }
        [Display(Name = "Nome")]
        public static string NomeExibicao { get; set; }
        [Display(Name = "Doc. Identificação")]
        public string DocumentoIdentificacao { get; set; }
        [Display(Name = "Nascimento")]
        public string DataNascimento { get; set; }

        public Usuario(IHttpContextAccessor acessor)
        {
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
                        NomeExibicao = Nome;
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
    }
}
