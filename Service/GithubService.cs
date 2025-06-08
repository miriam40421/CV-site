using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using Microsoft.Extensions.Options;

namespace Service
{
    public class GithubService : IGitHubServices
    {

        private readonly GitHubClient _client;
        private readonly GitHubIntegrationOptions _options;

        public GithubService(IOptions<GitHubIntegrationOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options), "GitHubIntegrationOptions cannot be null.");
            _client = new GitHubClient(new Octokit.ProductHeaderValue("my-github-app")) ?? throw new ArgumentNullException(nameof(options), "םםםפד");
            if (string.IsNullOrEmpty(_options.Token) || string.IsNullOrEmpty(_options.UserName))
            {
                throw new InvalidOperationException("GitHubIntegrationOptions values are not properly configured.");
            }
            if (!string.IsNullOrEmpty(_options.Token))
            {
                var tokenAuth = new Credentials(_options.Token); // הכנס את הטוקן שלך
                _client.Credentials = tokenAuth;
            }
        }


        public async Task<int> GetUserFollowersAsync(string userName)
        {

            var user = await _client.User.Get(userName);
            Console.WriteLine($"User: {user.Login}, Name: {user.Name}");

            return user.Followers;
        }

        public async Task<List<Repository>> SearchRepositoriesInCSharp()
        {
            var reqest = new SearchRepositoriesRequest("repo-name") { Language = Language.CSharp };
            var result = await _client.Search.SearchRepo(reqest);
            return result.Items.ToList();
        }


        public class RepositoryInfo
        {
            public string Name { get; set; }
            public List<string> Languages { get; set; } = new List<string>();// השאר כ-Dictionary
            public DateTimeOffset? LastCommitDate { get; set; }
            public int StarsCount { get; set; }
            public int PullRequestsCount { get; set; }
            public string Url { get; set; }
        }

        public async Task<IReadOnlyList<RepositoryLanguage>> GetAllLanguage(long Id)
        {
            Task<IReadOnlyList<RepositoryLanguage>> myLang = _client.Repository.GetAllLanguages(Id);
            IReadOnlyList<RepositoryLanguage> languages = await myLang;
            return languages;
        }


        public async Task<List<RepositoryInfo>> GetRepositoriesInfoAsync()
        {
            var repositories = await _client.Repository.GetAllForCurrent();
            var repositoryInfos = new List<RepositoryInfo>();

            foreach (var repo in repositories)
            {
                var pullRequests = await _client.PullRequest.GetAllForRepository(repo.Owner.Login, repo.Name);
                int pullRequestsCount = pullRequests.Count();
                IReadOnlyList<RepositoryLanguage> languages = await GetAllLanguage(repo.Id);
                Console.WriteLine(repositoryInfos);
                DateTimeOffset? lastCommitDate = repo.PushedAt;
                RepositoryInfo r = new RepositoryInfo
                {
                    Name = repo.Name,
                    LastCommitDate = lastCommitDate,
                    StarsCount = repo.StargazersCount,
                    PullRequestsCount = pullRequestsCount,
                    Url = repo.HtmlUrl
                };

                foreach (var lan in languages)
                {
                    Console.WriteLine(lan.Name);
                    r.Languages.Add(lan.Name);

                }
                repositoryInfos.Add(r);
            }

            return repositoryInfos;
        }
        public async Task<List<string>> SearchRepositories(string? repo, string? lan, string? userName)

        {
            IReadOnlyList<Repository> repositories1 = await _client.Repository.GetAllForCurrent();
            List<string> repositories2 = [];

            IReadOnlyList<Repository> repositories = await _client.Repository.GetAllForCurrent();
            if (userName != null)
            {
                repositories = await _client.Repository.GetAllForUser(userName);
                foreach (var item in repositories)
                {
                    Console.WriteLine(item.Name);
                }
                Console.WriteLine(repositories.ToArray());
                Console.WriteLine("ooo");
            }
            if (repo != null)
            {
                repositories = repositories.Where(x => x.Name == repo).ToList();
                foreach (var item in repositories)
                {
                    Console.WriteLine(item.Name);
                }
                Console.WriteLine(repositories.ToArray());
                Console.WriteLine("ooo");

            }


            if (lan != null)
            {

                foreach (var rep in repositories)
                {

                    IReadOnlyList<RepositoryLanguage> languages = await GetAllLanguage(rep.Id);
                    bool f = false;

                    foreach (var l in languages)
                    {
                        if (l.Name == lan)
                        {
                            f = true;
                        }
                    }
                    if (f)
                    {
                        repositories2.Append(rep.Name);
                    }
                }
            }

            if (repositories2.Count > 0)
            {
                return repositories2;

            }
            

            else
            {
                foreach (var item in repositories)
                {
                    repositories2.Add(item.Name);

                }

                    return repositories2;

            }
        }
    }
}








