using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SessionPresenceUpdater.Providers
{
    public class SessionDetailProvider
    {

        public async Task<SessionDetail> GetPlayerCountAsync(string sessionName)
        {

            string SessionEndPoint = $"http://zoo.schlo.moe/session/{sessionName}/";

            using (var client = new HttpClient())
            {
                HttpResponseMessage response;
                Stream content;
                SessionDetail data;


                response = await client.GetAsync(SessionEndPoint);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    // If we didn't get a valid response back, early exit.
                    return null;
                }

                content = await response.Content.ReadAsStreamAsync();

                data = DeserializeFromStream(content);

                return data;
            }
        }


        private SessionDetail DeserializeFromStream(Stream stream)
        {
            JsonSerializer serializer;


            serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<SessionDetail>(jsonTextReader);
            }
        }

    }

}
