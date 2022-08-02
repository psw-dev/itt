using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace psw.itt.common
{
    public class PSWHttpClient : IDisposable
    {
        private HttpClient client;

        public PSWHttpClient(string baseAddress)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void Dispose()
        {
            client.Dispose();
        }

        async public Task<string> PostRequest(string uri, string dataJson, string contentType)
        {
            try
            {
                string result = null;

                HttpResponseMessage resp = await client.PostAsync(uri, new StringContent(dataJson, System.Text.Encoding.UTF8, contentType));

                if (resp.IsSuccessStatusCode)
                {
                    result = await resp.Content.ReadAsStringAsync();
                }
                else
                    result = null;

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> GetRequest(string uri)
        {
            try
            {
                string result = null;

                HttpResponseMessage resp = await client.GetAsync(uri);
                if (resp.IsSuccessStatusCode)
                {
                    result = await resp.Content.ReadAsStringAsync();
                }
                else
                    result = null;

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception, GetRequest:  " + ex);
                return null;
            }
        }
    }
}