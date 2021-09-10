using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Extensions.Configuration;

namespace Identity
{
    public class Usuario
    {
        public string Token { get; set; }
        public string Login { get; set; }
        public string Nome { get; set; }
        public string DocumentoIdentificacao { get; set; }
        public string DataNascimento { get; set; }
        private IHttpContextAccessor Acessor { get; }
        private static IConfiguration Configuration { get; set; }
        private XmlDocument TokenSAML { get; set; }

        public Usuario(IHttpContextAccessor acessor, IConfiguration configuration)
        {
            Acessor = acessor;
            Configuration = configuration;
            TokenSAML = new XmlDocument();
            if (Acessor.HttpContext.User.Identities.First().BootstrapContext != null)
            {
                Token = Acessor.HttpContext.User.Identities.First().BootstrapContext.ToString();
                //Se BootstrapContext tem conteúdo, estamos no contexto da autenticação na aplicação
                TokenSAML.LoadXml(Token);
                Token = Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(Token));
            }
            else
            {
                //Se não, estamos tratando da autorização que chegou na requisição à api
                if (Acessor.HttpContext.Request.Headers["Authorization"].Count > 0)
                {
                    try
                    {
                        TokenSAML.LoadXml(UTF8Encoding.UTF8.GetString(Convert.FromBase64String(Acessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1])));
                    }
                    catch
                    {

                    }
                }
            }
            RegistrarPropriedades();
        }

        private void RegistrarPropriedades()
        {
            if (TokenSAML != null)
            {
                var _atributos = TokenSAML.GetElementsByTagName("saml:Attribute");
                var _ver1 = (_atributos.Count > 0);
                if (!_ver1) { _atributos = TokenSAML.GetElementsByTagName("Attribute"); }

                string _campo;
                string _valor;
                foreach (XmlNode _atributo in _atributos)
                {
                    if (_ver1)
                    {
                        _campo = _atributo.Attributes.GetNamedItem("AttributeName").InnerText;
                    }
                    else
                    {
                        _campo = _atributo.Attributes[0].Value;
                        _campo = _campo.Split("/")[_campo.Split("/").Count() - 1];
                    }

                    _valor = _atributo.InnerText;

                    if (_campo == "upn")
                        Login = _valor;
                    else
                    if (_campo == "name")
                    {
                        Nome = _valor.Split(":")[0];
                        if (_valor.Split(":").Count() > 1)
                        {
                            DocumentoIdentificacao = _valor.Split(":")[1];
                        }
                    }
                    else
                    if (_campo == "CNPJ")
                        DocumentoIdentificacao = _valor;
                    else
                    if (_campo == "CPF")
                        DocumentoIdentificacao = _valor;
                    else
                    if (_campo == "dateofbirth")
                        DataNascimento = _valor;
                };
            }
        }
    }
}
