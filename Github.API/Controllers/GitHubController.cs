using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using Service;

namespace Github.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GitHubController : ControllerBase
    {
        private readonly Service.IGitHubServices GithubService;

        public GitHubController(Service.IGitHubServices githubService)
        {
            GithubService = githubService ?? throw new ArgumentNullException(nameof(githubService));
        }
        [HttpGet]
        public async Task<int> GetUserFollowersAsync(string userName)
        {
            return await GithubService.GetUserFollowersAsync(userName);
        }

        //[HttpGet("repo")]
        //public async Task<List<Repository>> SearchRepositoriesInCSharp()
        //{
        //    return await GithubService.SearchRepositoriesInCSharp();
        //}
        [HttpGet("repo")]
        public async Task<List<GithubService.RepositoryInfo>> GetRepositoriesInfoAsync()
        {
            return await GithubService.GetRepositoriesInfoAsync();
        }
        [HttpGet("search")]
        public async Task<List<string>> SearchRepositories(string? repo, string? lan, string? userName)
        {
            return await GithubService.SearchRepositories(repo, lan, userName);
        }



    
}
}
