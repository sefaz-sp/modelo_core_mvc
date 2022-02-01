using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using modelo_core_mvc.projetos;
using modelo_core_mvc.Models;

namespace modelo_core_mvc.HttpClients
{
    public class ProjetosApiClient
    {
        private readonly HttpClient _httpClient;
        private IConfiguration _configuration;

        public ProjetosApiClient(HttpClient httpClient, IConfiguration configuration, Usuario usuario)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new System.Uri(_configuration["apiendereco:projetos"]);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", usuario.Token);
        }

        //Consultar
        public async Task<Projetos> GetProjetoAsync(long cd_projeto)
        {
            var resposta = await _httpClient.GetAsync($"Projetos/{cd_projeto}");
            resposta.EnsureSuccessStatusCode();
            return new Projetos().ToModel(await resposta.Content.ReadAsStringAsync());
        }

        //Listar todos
        public async Task<IEnumerable<Projetos>> GetProjetosAsync()
        {
            var resposta = await _httpClient.GetAsync($"Projetos");
            resposta.EnsureSuccessStatusCode();
            return new Projetos().ToList(await resposta.Content.ReadAsStringAsync());
        }

        //Verificar api
        public async Task<string> GetStatusAsync()
        {
            var resposta = await _httpClient.GetAsync($"projetos/status");
            resposta.EnsureSuccessStatusCode();
            return await resposta.Content.ReadAsStringAsync();
        }

        //Verificar conexão
        public async Task<string> GetConexaoAsync()
        {
            var resposta = await _httpClient.GetAsync($"projetos/conexao");
            resposta.EnsureSuccessStatusCode();
            return await resposta.Content.ReadAsStringAsync();
        }

        public async Task DeleteProjetoAsync(long cd_projeto)
        {
            if (cd_projeto != 0)
            {
                var resposta = await _httpClient.DeleteAsync($"Projetos/{cd_projeto}");
                resposta.EnsureSuccessStatusCode();
            }
        }

        //Incluir
        public async Task PostProjetoAsync(Projetos projeto)
        {
            var resposta = await _httpClient.PostAsync("Projetos", projeto.ToJson());
            resposta.EnsureSuccessStatusCode();
        }

        //Alterar
        public async Task PutProjetoAsync(Projetos projeto)
        {
            var resposta = await _httpClient.PutAsync("Projetos", projeto.ToJson());
            resposta.EnsureSuccessStatusCode();
        }

        public async Task<byte[]> GetAnexoAsync(long cd_projeto)
        {
            var resposta = await _httpClient.GetAsync($"Projetos/{cd_projeto}/anexo");
            resposta.EnsureSuccessStatusCode();
            return await resposta.Content.ReadAsByteArrayAsync();
        }
    }
}
