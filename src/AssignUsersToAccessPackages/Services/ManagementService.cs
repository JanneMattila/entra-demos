using AssignUsersToAccessPackages.Models;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Beta;
using Microsoft.Graph.Beta.IdentityGovernance.LifecycleWorkflows.Workflows.Item.MicrosoftGraphIdentityGovernanceActivate;
using Microsoft.Graph.Beta.Models;

namespace AssignUsersToAccessPackages.Services;

public class ManagementService
{
    private readonly GraphServiceClient _client;
    private readonly string _query;

    public ManagementService(IOptions<ManagementOptions> options)
    {
        var scopes = new[] { "https://graph.microsoft.com/.default" };
        var clientSecretCredential = new ClientSecretCredential(
            options.Value.TenantId, options.Value.ClientId, options.Value.ClientSecret);

        _query = options.Value.Query;
        _client = new GraphServiceClient(clientSecretCredential, scopes);
    }

    public async Task<List<DataModel>> GetUsers()
    {
        var result = await _client.Users.GetAsync(request =>
        {
            request.QueryParameters.Filter = _query;
        });

        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(result.Value);

        return result.Value.Select(u => new DataModel
        {
            ID = u.Id,
            Name = u.DisplayName
        }).ToList();
    }

    public async Task<List<DataModel>> GetAccessPackages()
    {
        var result = await _client.IdentityGovernance.LifecycleWorkflows.Workflows.GetAsync(request =>
        {
        });

        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(result.Value);

        return result.Value.Select(u => new DataModel
        {
            ID = u.Id,
            Name = u.DisplayName
        }).ToList();
    }

    public async Task Assign(List<string> users, List<string> accesses)
    {
        var requestBody = new ActivatePostRequestBody
        {
            Subjects = users.Select(u => new User
            {
                Id = u
            }).ToList()
        };

        foreach (var access in accesses)
        {
            await _client.IdentityGovernance.LifecycleWorkflows.Workflows[access].MicrosoftGraphIdentityGovernanceActivate.PostAsync(requestBody);
        }
    }
}
