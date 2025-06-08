using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Octokit;
using Service;
using static Service.GithubService;

namespace Github.API.CachedServices
{
     public class CachedServices : IGitHubServices
    {
        private readonly IGitHubServices _gitHubServices;
        private readonly IMemoryCache _memoryCache;
        private const string userProfileKey = "userProfileKey";
        private DateTimeOffset LastUpdate;
        public readonly GitHubClient _client;
        public readonly GitHubIntegrationOptions _option;


        public CachedServices(IGitHubServices gitHubService, IMemoryCache memoryCache, GitHubClient client, IOptions<GitHubIntegrationOptions> option)
        {
            _gitHubServices = gitHubService;
            _memoryCache = memoryCache;
            _client = client;
            _option = option.Value;
            try
            {
                Credentials tokenAuth = null;
                if (_option.Token is not null)
                {
                    tokenAuth = new Credentials(_option.Token);

                }
                _client.Credentials = tokenAuth;
            }
            catch (Exception ex)

            {
                throw new Exception("Failed to create GitHubClient.", ex);

            }
        }



        //public CachedServices(IGitHubServices gitHubServices, IMemoryCache memoryCache, GitHubClient client, IOptions<GitHubIntegrationOptions> option)
        //{
        //    _gitHubServices = gitHubServices ?? throw new ArgumentNullException(nameof(gitHubServices), "IGitHubServices cannot be null.");
        //    _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache), "IMemoryCache cannot be null.");
        //    _client = client ?? throw new ArgumentNullException(nameof(client), "GitHubClient cannot be null.");
        //    _option = option.Value ?? throw new ArgumentNullException(nameof(option), "IOptions cannot be null.");
        //}

        //public async Task<List<RepositoryInfo>> GetRepositoriesInfoAsync()
        //{
        //    DateTimeOffset today = new DateTimeOffset();
        //    //DateTimeOffset mostDate = new DateTimeOffset();

        //    if (_memoryCache.TryGetValue(userProfileKey, out List<RepositoryInfo> portfolio))
        //    {
        //        if (_memoryCache.TryGetValue(LastUpdate, out DateTimeOffset lastSave))
        //        {
        //            IReadOnlyList<Activity> act = await _client.Activity.Events.GetAllUserPerformed(_option.UserName);
        //            //bool flag = false;
        //            bool hasNewActivity = act.Any(item => item.CreatedAt.CompareTo(lastSave) > 0);

        //            //foreach (var item in act)
        //            //{
        //                //int c = item.CreatedAt.CompareTo(lastSave);
        //                //if (c >0)
        //                //{
        //                    //flag = true;
        //                //}
        //            //}
        //            if (hasNewActivity)
        //            {
        //                MemoryCacheEntryOptions cache = new MemoryCacheEntryOptions();

        //                portfolio = await _gitHubServices.GetRepositoriesInfoAsync();
        //                _memoryCache.Set(userProfileKey, portfolio, cache);
        //                MemoryCacheEntryOptions cacheDate = new MemoryCacheEntryOptions();
        //                _memoryCache.Set(LastUpdate, today, cacheDate);
        //            }
        //            else
        //                return portfolio;
        //        }

        //    }
        //    var cacheOption = new MemoryCacheEntryOptions();
        //        //.SetAbsoluteExpiration(TimeSpan.FromSeconds(300))
        //        //.SetSlidingExpiration(TimeSpan.FromSeconds(100));
        //    portfolio = await _gitHubServices.GetRepositoriesInfoAsync();
        //    _memoryCache.Set(userProfileKey, portfolio, cacheOption);
        //    MemoryCacheEntryOptions cacheDate1 = new MemoryCacheEntryOptions();
        //    _memoryCache.Set(LastUpdate, today, cacheDate1);
        //    return portfolio;
        //}

        public async Task<List<RepositoryInfo>> GetRepositoriesInfoAsync()
        {
            DateTimeOffset today = DateTimeOffset.UtcNow;

            if (_memoryCache.TryGetValue(userProfileKey, out List<RepositoryInfo> portfolio))
            {
                if (_memoryCache.TryGetValue(LastUpdate, out DateTimeOffset lastSave))
                {
                    IReadOnlyList<Activity> act = await _client.Activity.Events.GetAllUserPerformed(_option.UserName);
                    bool hasNewActivity = act.Any(item => item.CreatedAt.CompareTo(lastSave) > 0);

                    if (hasNewActivity)
                    {
                        portfolio = await _gitHubServices.GetRepositoriesInfoAsync();
                        _memoryCache.Set(userProfileKey, portfolio);
                        _memoryCache.Set(LastUpdate, today);
                    }
                    else
                    {
                        return portfolio;
                    }
                }
            }

            // אם לא נמצא במטמון או שאין עדכונים, נטען מחדש
            portfolio = await _gitHubServices.GetRepositoriesInfoAsync();
            _memoryCache.Set(userProfileKey, portfolio);
            _memoryCache.Set(LastUpdate, today);

            return portfolio;
        }









        //public async Task<List<GithubService.RepositoryInfo>> GetRepositoriesInfoAsync()
        //{
        //    if (_memoryCache.TryGetValue(userProfileKey, out List<GithubService.RepositoryInfo> repositoryInfo))
        //        return repositoryInfo;

        //    var cacheOption = new MemoryCacheEntryOptions()
        //        .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
        //        .SetSlidingExpiration(TimeSpan.FromSeconds(10));

        //    repositoryInfo = await _gitHubServices.GetRepositoriesInfoAsync();
        //    _memoryCache.Set(userProfileKey, repositoryInfo, cacheOption);
        //    return repositoryInfo;
        //}

        public Task<int> GetUserFollowersAsync(string userName)
        {
            return _gitHubServices.GetUserFollowersAsync(userName);
        }

        public async Task<List<string>> SearchRepositories(string? repo, string? lan, string? userName)
        {
            if (_memoryCache.TryGetValue(userProfileKey, out List<string> repository))
                return repository;

            var cacheOption = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
                .SetSlidingExpiration(TimeSpan.FromSeconds(10));

            repository = await _gitHubServices.SearchRepositories(repo, lan, userName);
            _memoryCache.Set(userProfileKey, repository, cacheOption);
            return repository;
        }

        public async Task<List<Repository>> SearchRepositoriesInCSharp()
        {
            return await _gitHubServices.SearchRepositoriesInCSharp();
        }

     

   
    }

}
