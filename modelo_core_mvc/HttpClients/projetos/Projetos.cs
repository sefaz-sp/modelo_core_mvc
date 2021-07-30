using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace modelo_core_mvc.projetos
{
    public class Projetos
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public Projetos(int id, string nome, string descricao)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
        }

        public Projetos()
        {

        }

        public StringContent ToJson()
        {
            return new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
        }

        public Projetos ToModel(string ProjetoJson)
        {
            return JsonConvert.DeserializeObject<Projetos>(ProjetoJson);
        }

        public IEnumerable<Projetos> ToList(string ProjetoJson)
        {
            return JsonConvert.DeserializeObject<IEnumerable<Projetos>>(ProjetoJson);
        }
    }
}
