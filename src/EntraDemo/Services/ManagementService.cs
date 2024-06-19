using Azure.Identity;
using EntraDemo.Models;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Beta;
using Microsoft.Graph.Beta.IdentityGovernance.LifecycleWorkflows.Workflows.Item.MicrosoftGraphIdentityGovernanceActivate;
using Microsoft.Graph.Beta.Models;
using System.Security.Cryptography;
using System.Text;

namespace EntraDemo.Services;

public class ManagementService
{
    private readonly GraphServiceClient _client;
    private readonly string _userQuery;
    private readonly string _accessQuery;
    private readonly string _domain;

    public ManagementService(IOptions<ManagementOptions> options)
    {
        var scopes = new[] { "https://graph.microsoft.com/.default" };
        var clientSecretCredential = new ClientSecretCredential(
            options.Value.TenantId, options.Value.ClientId, options.Value.ClientSecret);

        _userQuery = options.Value.UserQuery;
        _accessQuery = options.Value.AccessQuery;
        _client = new GraphServiceClient(clientSecretCredential, scopes);
        _domain = options.Value.Domain;
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

        return result.Value.Where(o => o.Description.Contains(_accessQuery)).Select(u => new DataModel
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
    public async Task<UserModel> GetUser(string id)
    {
        var users = await GetUsers();
        if (!users.Any(u => u.ID == id))
        {
            throw new ArgumentException("User not found", nameof(id));
        }

        var user = await _client.Users[id].GetAsync();

        ArgumentNullException.ThrowIfNull(user);

        return new UserModel
        {
            ID = user.Id,
            DisplayName = user.DisplayName,
            FirstName = user.GivenName,
            LastName = user.Surname,
            JobTitle = user.JobTitle,
            UserPrincipalName = user.UserPrincipalName,
            Department = user.Department,
            AccountEnabled = user.AccountEnabled.GetValueOrDefault()
        };
    }

    public static string GeneratePassword(int length)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
        var randomBytes = new byte[length];

        RandomNumberGenerator.Fill(randomBytes);

        var result = new StringBuilder(length);
        foreach (var randomByte in randomBytes)
        {
            result.Append(validChars[randomByte % validChars.Length]);
        }

        return result.ToString();
    }

    public async Task<string> ManageUser(UserModel user)
    {
        if (string.IsNullOrEmpty(user.ID))
        {
            var password = GeneratePassword(12);
            var newUser = new User
            {
                DisplayName = $"{user.FirstName} {user.LastName}",
                UserPrincipalName = $"{user.UserPrincipalName}@{_domain}",
                PasswordProfile = new PasswordProfile
                {
                    Password = password,
                    ForceChangePasswordNextSignIn = true
                },
                MailNickname = user.UserPrincipalName,
                GivenName = user.FirstName,
                Surname = user.LastName,
                JobTitle = user.JobTitle,
                Department = user.Department,
                AccountEnabled = user.AccountEnabled
            };

            await _client.Users.PostAsync(newUser);
            return password;
        }
        else
        {
            var users = await GetUsers();
            if (!users.Any(u => u.ID == user.ID))
            {
                throw new ArgumentException("UserProperties not found", nameof(user.ID));
            }

            var existingUser = await _client.Users[user.ID].GetAsync();
            if (existingUser != null)
            {
                existingUser.DisplayName = user.DisplayName;
                existingUser.GivenName = user.FirstName;
                existingUser.Surname = user.LastName;
                existingUser.JobTitle = user.JobTitle;
                existingUser.Department = user.Department;
                existingUser.AccountEnabled = user.AccountEnabled;

                await _client.Users[user.ID].PatchAsync(existingUser);
            }
        }
        return string.Empty;
    }

    public async Task RemoveUser(string id)
    {
        await _client.Users[id].DeleteAsync();
    }
}
