using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace modelo_core_mvc.Models
{
    public class Usuario
    {
        public string Token { get; set; }
        public string Login { get; set; }
        public static string Nome { get; set; }
        public string DocumentoIdentificacao { get; set; }
        public string DataNascimento { get; set; }
        private IHttpContextAccessor Acessor { get; }

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
                    if (campo == "upn")
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
    }
}
