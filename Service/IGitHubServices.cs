using Octokit;

namespace Service
{
    public interface IGitHubServices
    {
        public Task<int> GetUserFollowersAsync(string userName);
        public Task<List<Repository>> SearchRepositoriesInCSharp();
        public Task<List<GithubService.RepositoryInfo>> GetRepositoriesInfoAsync();
        public Task<List<string>> SearchRepositories(string? repo, string? lan, string? userName);

    }
}
