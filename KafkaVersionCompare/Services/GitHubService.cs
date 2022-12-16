using Octokit;

namespace KafkaVersionCompare.Services;

public class GitHubService
{
    public const string UserAgent = "KafkaVersionCompare";
    public const string RepositoryOwner = "apache";
    public const string Repository = "kafka";

    private readonly GitHubClient _gitHubClient;


    public GitHubService(GitHubClient gitHubClient)
    {
        _gitHubClient = gitHubClient;

    }

    public async Task<IReadOnlyList<RepositoryContributor>> GetContributorsAsync()
    {
        return await _gitHubClient.Repository.GetAllContributors(RepositoryOwner, Repository);
       
    }

    public async Task<IReadOnlyList<Label>> GetLabelsAsync()
    {
        return await _gitHubClient.Issue.Labels.GetAllForRepository(RepositoryOwner, Repository);
    }

}