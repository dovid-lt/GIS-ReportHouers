using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GetHouers
{

    class DownloadData
    {


        public async Task<IEnumerable<List<string>>> getRows(int year, int month, Settings settings)
        {
            var clientHandler = new HttpClientHandler() { UseProxy = true };
            clientHandler.AllowAutoRedirect = false;

            using var client = new HttpClient(clientHandler);

            var nameAndPass = new[] {
                new KeyValuePair<string, string>("username", settings.UserName),
                new KeyValuePair<string, string>("password", settings.Password)
            };

            await client.PostAsync("https://cloud.phonecall.co/pbx/login.php", new FormUrlEncodedContent(nameAndPass));

            var citers = new Criters
            {
                groupOp = "AND",
                rules = new List<Rule>
            {
                new Rule { field = "iv_name", op = "eq", data = settings.FieldNameType},
                new Rule { field = "il_cdr_start", op = "bw", data = $"{year}-{month:d2}-"},
                new Rule { field = "il_action", op = "eq", data = "SELECT"}
            }
            };

            var list = new List<IEnumerable<List<string>>>();
            var page = 1;

            while (true)
            {

                var dic = new Dictionary<string, string>
                {
                    {"_search","true"},
                    {"rows","500"},
                    {"page", page.ToString()},
                    {"sidx","il_cdr_start"},
                    {"sord","asc"},
                    {"filters", JsonSerializer.Serialize(citers)}
                };


                var response = await client.PostAsync("https://cloud.phonecall.co/pbx/ivrlogs_json.php", new FormUrlEncodedContent(dic));
                var result = await response.Content.ReadAsStreamAsync();

                var obj = await JsonSerializer.DeserializeAsync<Dictionary<string, dynamic>>(result);
                var rows = (((JsonElement)obj["rows"]).EnumerateArray().Select(wl => wl.GetProperty("cell").EnumerateArray().Select(x => x.ToString()).ToList()));
                list.Add(rows);

                var pages = ((JsonElement)obj["page"]).GetInt32();
                if (pages <= page)
                    break;
            }

            return list.SelectMany(item => item);
        }




        class Criters
        {
            public string groupOp { get; set; }
            public List<Rule> rules { get; set; }
        }

        class Rule
        {
            public string field { get; set; }
            public string op { get; set; }
            public string data { get; set; }
        }
    }
}
