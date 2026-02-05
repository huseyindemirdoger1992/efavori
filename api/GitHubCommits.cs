using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace api
{
    // GitHub API Settings Model
    public class GitHubSettings
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string RepoName { get; set; }
    }

    // Commit Author/Committer Model
    public class GitHubUser
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
    }

    // Commit Detail Model
    public class CommitDetail
    {
        [JsonPropertyName("author")]
        public GitHubUser Author { get; set; }

        [JsonPropertyName("committer")]
        public GitHubUser Committer { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("tree")]
        public TreeInfo Tree { get; set; }

        [JsonPropertyName("comment_count")]
        public int CommentCount { get; set; }

        [JsonPropertyName("verification")]
        public VerificationInfo Verification { get; set; }
    }

    // Tree Information
    public class TreeInfo
    {
        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    // Verification Information
    public class VerificationInfo
    {
        [JsonPropertyName("verified")]
        public bool Verified { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        [JsonPropertyName("payload")]
        public string Payload { get; set; }
    }

    // GitHub User Profile (for commit author/committer)
    public class GitHubProfile
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    // Parent Commit Information
    public class ParentCommit
    {
        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }
    }

    // File Changes in Commit
    public class FileChange
    {
        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("filename")]
        public string Filename { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("additions")]
        public int Additions { get; set; }

        [JsonPropertyName("deletions")]
        public int Deletions { get; set; }

        [JsonPropertyName("changes")]
        public int Changes { get; set; }

        [JsonPropertyName("blob_url")]
        public string BlobUrl { get; set; }

        [JsonPropertyName("raw_url")]
        public string RawUrl { get; set; }

        [JsonPropertyName("contents_url")]
        public string ContentsUrl { get; set; }

        [JsonPropertyName("patch")]
        public string Patch { get; set; }
    }

    // Commit Stats
    public class CommitStats
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("additions")]
        public int Additions { get; set; }

        [JsonPropertyName("deletions")]
        public int Deletions { get; set; }
    }

    // Main Commit Model
    public class GitHubCommit
    {
        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }

        [JsonPropertyName("commit")]
        public CommitDetail Commit { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [JsonPropertyName("comments_url")]
        public string CommentsUrl { get; set; }

        [JsonPropertyName("author")]
        public GitHubProfile Author { get; set; }

        [JsonPropertyName("committer")]
        public GitHubProfile Committer { get; set; }

        [JsonPropertyName("parents")]
        public List<ParentCommit> Parents { get; set; }

        [JsonPropertyName("stats")]
        public CommitStats Stats { get; set; }

        [JsonPropertyName("files")]
        public List<FileChange> Files { get; set; }
    }

    // Main GitHubCommits Service Class
    public class GitHubCommits
    {
        private readonly HttpClient _httpClient;
        private readonly GitHubSettings _settings;

        public GitHubCommits(IConfiguration configuration)
        {
            _settings = new GitHubSettings();
            configuration.GetSection("GitHubSettings").Bind(_settings);

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GitHubCommitsApp", "1.0"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        }

        // Tüm commit'leri çek (liste halinde)
        public async Task<List<GitHubCommit>> GetAllCommitsAsync(int perPage = 100, int page = 1)
        {
            try
            {
                var url = $"https://api.github.com/repos/{_settings.UserName}/{_settings.RepoName}/commits?per_page={perPage}&page={page}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var commits = JsonSerializer.Deserialize<List<GitHubCommit>>(json);

                return commits ?? new List<GitHubCommit>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting commits: {ex.Message}");
                throw;
            }
        }

        // Belirli bir commit'in detaylarını çek
        public async Task<GitHubCommit> GetCommitByIdAsync(string commitSha)
        {
            try
            {
                var url = $"https://api.github.com/repos/{_settings.UserName}/{_settings.RepoName}/commits/{commitSha}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var commit = JsonSerializer.Deserialize<GitHubCommit>(json);

                return commit;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting commit {commitSha}: {ex.Message}");
                throw;
            }
        }

        // Belirli bir branch'teki commit'leri çek
        public async Task<List<GitHubCommit>> GetCommitsByBranchAsync(string branch, int perPage = 100, int page = 1)
        {
            try
            {
                var url = $"https://api.github.com/repos/{_settings.UserName}/{_settings.RepoName}/commits?sha={branch}&per_page={perPage}&page={page}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var commits = JsonSerializer.Deserialize<List<GitHubCommit>>(json);

                return commits ?? new List<GitHubCommit>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting commits for branch {branch}: {ex.Message}");
                throw;
            }
        }

        // Belirli bir tarih aralığındaki commit'leri çek
        public async Task<List<GitHubCommit>> GetCommitsByDateRangeAsync(DateTime since, DateTime until, int perPage = 100, int page = 1)
        {
            try
            {
                var sinceStr = since.ToString("yyyy-MM-ddTHH:mm:ssZ");
                var untilStr = until.ToString("yyyy-MM-ddTHH:mm:ssZ");
                var url = $"https://api.github.com/repos/{_settings.UserName}/{_settings.RepoName}/commits?since={sinceStr}&until={untilStr}&per_page={perPage}&page={page}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var commits = JsonSerializer.Deserialize<List<GitHubCommit>>(json);

                return commits ?? new List<GitHubCommit>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting commits by date range: {ex.Message}");
                throw;
            }
        }

        // Belirli bir author'a ait commit'leri çek
        public async Task<List<GitHubCommit>> GetCommitsByAuthorAsync(string author, int perPage = 100, int page = 1)
        {
            try
            {
                var url = $"https://api.github.com/repos/{_settings.UserName}/{_settings.RepoName}/commits?author={author}&per_page={perPage}&page={page}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var commits = JsonSerializer.Deserialize<List<GitHubCommit>>(json);

                return commits ?? new List<GitHubCommit>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting commits by author {author}: {ex.Message}");
                throw;
            }
        }

        // Belirli bir path'teki dosyaların commit'lerini çek
        public async Task<List<GitHubCommit>> GetCommitsByPathAsync(string path, int perPage = 100, int page = 1)
        {
            try
            {
                var url = $"https://api.github.com/repos/{_settings.UserName}/{_settings.RepoName}/commits?path={path}&per_page={perPage}&page={page}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var commits = JsonSerializer.Deserialize<List<GitHubCommit>>(json);

                return commits ?? new List<GitHubCommit>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting commits by path {path}: {ex.Message}");
                throw;
            }
        }

        // Commit istatistiklerini al
        public CommitStatistics GetCommitStatistics(List<GitHubCommit> commits)
        {
            return new CommitStatistics
            {
                TotalCommits = commits.Count,
                TotalAdditions = commits.Where(c => c.Stats != null).Sum(c => c.Stats.Additions),
                TotalDeletions = commits.Where(c => c.Stats != null).Sum(c => c.Stats.Deletions),
                TotalChanges = commits.Where(c => c.Stats != null).Sum(c => c.Stats.Total),
                UniqueAuthors = commits.Where(c => c.Author != null).Select(c => c.Author.Login).Distinct().Count(),
                FirstCommitDate = commits.OrderBy(c => c.Commit.Author.Date).FirstOrDefault()?.Commit.Author.Date,
                LastCommitDate = commits.OrderByDescending(c => c.Commit.Author.Date).FirstOrDefault()?.Commit.Author.Date
            };
        }

        // Dispose
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    // İstatistik modeli
    public class CommitStatistics
    {
        public int TotalCommits { get; set; }
        public int TotalAdditions { get; set; }
        public int TotalDeletions { get; set; }
        public int TotalChanges { get; set; }
        public int UniqueAuthors { get; set; }
        public DateTime? FirstCommitDate { get; set; }
        public DateTime? LastCommitDate { get; set; }
    }
}
