# GitHub User Activity CLI

`URL from the project task`: https://roadmap.sh/projects/github-user-activity

`GitHub-UA-CLI` is a command-line tool written in C# that allows you to retrieve and display a GitHub user's latest activity, such as events like starring repositories, pushing commits, creating repositories, forking, and more.

## Features

- Fetches user events from the GitHub API.
- Displays actions like watching, pushing commits, creating, deleting, forking repositories, and more.
- Handles common errors, such as invalid usernames or lack of activity.

## Requirements

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- Internet connection for making requests to the GitHub API.

## Installation

1. **Clone the repository** or download the project source files.

    ```bash
    git clone https://github.com/your-username/github-ua-cli.git
    ```

2. **Navigate to the project folder**.

    ```bash
    cd github-ua-cli
    ```

3. **Build the project**.

    ```bash
    dotnet build
    ```

## How to Run

To run the CLI, you need to provide a GitHub username as an argument:

```bash
dotnet run -- <GitHubUsername>

