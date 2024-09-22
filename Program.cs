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
            try
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
                    string username = args[0].Trim();
                    url = $"https://api.github.com/users/{username}/events";
                }

                var jsonDocument = await GetUserEvents(client, url);

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
                            if (issueAction != null)
                            {
                                action = $"{char.ToUpper(issueAction[0])}{issueAction[1..]} an issue in {repoName}";
                            }
                            break;
                        case "IssueCommentEvent":
                            var issueCommentAction = payload.GetProperty("action").GetString();
                            if (issueCommentAction != null)
                            {
                                action = $"{char.ToUpper(issueCommentAction[0])}{issueCommentAction[1..]} an issue comment in {repoName}";
                            }
                            break;
                        case "PullRequestEvent":
                            var pullRequestAction = payload.GetProperty("action").GetString();
                            if (pullRequestAction != null)
                            {
                                action = $"{char.ToUpper(pullRequestAction[0])}{pullRequestAction[1..]} a pull request in {repoName}";
                            }
                            break;
                        case "PublicEvent":
                            action = $"Made {repoName} public";
                            break;
                        case "ReleaseEvent":
                            var releaseAction = payload.GetProperty("action").GetString();
                            if (releaseAction != null)
                            {
                                action = $"{char.ToUpper(releaseAction[0])}{releaseAction[1..]} a release in {repoName}";
                            }
                            break;
                        default:
                            action = "Undefined action";
                            break;
                    }

                    Console.WriteLine($"* {action}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        private static async Task<JsonDocument> GetUserEvents(HttpClient client, string url)
        {
            try
            {

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception("Invalid username - try again.");
                }

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new Exception("API Error - check request headers");
                }

                string responseBody = await response.Content.ReadAsStringAsync();

                var jsonDocument = JsonDocument.Parse(responseBody);
                var jsonFormatted = JsonSerializer.Serialize(jsonDocument);

                if (jsonFormatted == "[]")
                {
                    throw new Exception("User has no registered activity");
                }

                return jsonDocument;
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Request error: {e.Message}");
            }
        }
    }
}
