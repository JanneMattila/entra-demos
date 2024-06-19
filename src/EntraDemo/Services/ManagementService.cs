using Azure.Identity;
using EntraDemo.Models;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Beta;
using Microsoft.Graph.Beta.IdentityGovernance.LifecycleWorkflows.Workflows.Item.MicrosoftGraphIdentityGovernanceActivate;
using Microsoft.Graph.Beta.Models;

namespace EntraDemo.Services;

public class ManagementService
{
    private readonly GraphServiceClient _client;
    private readonly string _userQuery;
    private readonly string _accessQuery;

    public ManagementService(IOptions<ManagementOptions> options)
    {
        var scopes = new[] { "https://graph.microsoft.com/.default" };
        var clientSecretCredential = new ClientSecretCredential(
            options.Value.TenantId, options.Value.ClientId, options.Value.ClientSecret);

        _userQuery = options.Value.UserQuery;
        _accessQuery = options.Value.AccessQuery;
        _client = new GraphServiceClient(clientSecretCredential, scopes);
    }

    public async Task<List<DataModel>> GetUsers()
    {
        var result = await _client.Users.GetAsync(request =>
        {
            request.QueryParameters.Filter = _userQuery;
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

        return result.Value.Where(o => o.Description.Contains(_accessQuery)) .Select(u => new DataModel
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
