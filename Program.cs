namespace GitHub_UA_CLI
{
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("GitHub-UA-CLI");
            string username = "";
            string url = "";
            const int minArgs = 1;
            const int maxArgs = 1;

            if (args.Length == 0 || args.Length > 1) 
            {
                Console.WriteLine($"You should provide at least {minArgs} " 
                    + $"argument(s) but no more than {maxArgs} arguments.");
            }   
            else
            {
                username = args[0];
                url = $"https://api.github.com/users/{username}/events";
            }

            try
            {
                
                HttpResponseMessage response = await client.GetAsync(url);

                // response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var jsonDocument = JsonDocument.Parse(responseBody);
                var formattedJson = JsonSerializer.Serialize(jsonDocument, new JsonSerializerOptions { WriteIndented = true });
                
                foreach ( var item in jsonDocument.RootElement.EnumerateArray() ) 
                {
                    var type = item.GetProperty("type").GetString();
                    var action = string.Empty;
                    var repoName = item.GetProperty("repo").GetProperty("name").GetString();
                    Console.WriteLine($"{type}" + " " + $"{repoName}");
                }

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
        }
    }
}