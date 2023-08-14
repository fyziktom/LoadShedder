using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadShedderSimulator
{
    internal class Simulator
    {
        public string Id { get; set; } = "test";
        public int[] Board { get; set; } = new int[16];

        public string Url { get; set; } = "https://localhost:7056";
        public string ApiCommand { get; set; } = "/api/NewDeviceData";

        HttpClient client = new HttpClient();

        public async Task<string> SendData()
        {
            var obj = new
            {
                id = Id,
                data = Board
            };

            var cnt = JsonConvert.SerializeObject(obj);

            using (var content = new StringContent(cnt, System.Text.Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage result = await client.PostAsync(Url.Trim('/') + "/" + ApiCommand.TrimStart('/'), content);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var returnStr = await result.Content.ReadAsStringAsync();

                    if (returnStr != null)
                        return returnStr;
                    else
                        return "ERROR:Cannot read return message.";
                }
            }

            return string.Empty;
        }
    }
}
