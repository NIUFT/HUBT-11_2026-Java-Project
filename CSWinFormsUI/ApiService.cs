using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSWinFormsUI
{
    public static class ApiService
    {
        private static readonly HttpClient Client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:8080/"),
            Timeout = TimeSpan.FromSeconds(15)
        };

        public static async Task<List<T>> GetListAsync<T>(string relativeUrl)
        {
            HttpResponseMessage resp = await Client.GetAsync(relativeUrl);
            resp.EnsureSuccessStatusCode();
            string json = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        }

        public static async Task PostAsync<T>(string relativeUrl, T payload)
        {
            string json = JsonConvert.SerializeObject(payload);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage resp = await Client.PostAsync(relativeUrl, content);
                resp.EnsureSuccessStatusCode();
            }
        }

        public static async Task PutAsync<T>(string relativeUrl, T payload)
        {
            string json = JsonConvert.SerializeObject(payload);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage resp = await Client.PutAsync(relativeUrl, content);
                resp.EnsureSuccessStatusCode();
            }
        }

        public static async Task DeleteAsync(string relativeUrl)
        {
            HttpResponseMessage resp = await Client.DeleteAsync(relativeUrl);
            resp.EnsureSuccessStatusCode();
        }
    }
}
