namespace GitHub_UA_CLI
{
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;


    internal class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("GitHub-UA-CLI");
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
                string username = args[0];
                url = $"https://api.github.com/users/{username}/events";
            }

            try
            {

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Invalid username, try again.");
                    return;
                }

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("API Error, check UserAgent please.");
                    return;
                }

                string responseBody = await response.Content.ReadAsStringAsync();

                var jsonDocument = JsonDocument.Parse(responseBody);

                foreach (var item in jsonDocument.RootElement.EnumerateArray())
                {
                    var type = item.GetProperty("type").GetString();
                    var action = string.Empty;
                    var repoName = item.GetProperty("repo").GetProperty("name").GetString();
                    var payload = item.GetProperty("payload");

                    switch (type)
                    {
                        case "WatchEvent":
                            action = $"Starred {repoName}";
                            break;
                        case "PushEvent":
                            var numberCommits = payload.GetProperty("size");
                            action = $"Pushed {numberCommits} commits to {repoName}";
                            break;
                        case "CreateEvent":
                            action = $"Created {repoName}";
                            break;
                        case "DeleteEvent":
                            action = $"Deleted {repoName}";
                            break;
                        case "ForkEvent":
                            action = $"Forked {repoName}";
                            break;
                        case "IssuesEvent":
                            var issueAction = payload.GetProperty("action").GetString();
                            action = $"{char.ToUpper(issueAction[0])} {issueAction[1..]} an issue in {repoName}";
                            break;
                        case "PullRequestEvent":
                            var pullRequestAction = payload.GetProperty("action").GetString();
                            action = $"{char.ToUpper(pullRequestAction[0])} {pullRequestAction[1..]} a pull request in {repoName}";
                            break;
                        case "PublicEvent":
                            action = $"Made {repoName} public";
                            break;
                        default:
                            action = "Undefined action";
                            break;
                    }

                    Console.WriteLine($"* {action}");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
        }
    }
}