using AssignUsersToAccessPackages.Models;
using AssignUsersToAccessPackages.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace AssignUsersToAccessPackages.Pages;

[AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
public class IndexModel(ILogger<IndexModel> logger, GraphServiceClient graphServiceClient, ManagementService managementService) : PageModel
{
    private readonly GraphServiceClient _graphServiceClient = graphServiceClient;
    private readonly ManagementService _managementService = managementService;
    private readonly ILogger<IndexModel> _logger = logger;

    public string? DisplayName { get; set; }
    public List<DataModel> Users { get; set; } = [];
    public List<DataModel> AccessPackages { get; set; } = [];

    [BindProperty]
    public List<string> SelectedUsers { get; set; } = [];

    [BindProperty]
    public List<string> SelectedAccesses { get; set; } = [];

    public async Task OnGet()
    {
        var user = await _graphServiceClient.Me.GetAsync();

        DisplayName = user?.DisplayName;
        Users = await _managementService.GetUsers();
        AccessPackages = await _managementService.GetAccessPackages();
    }

    public async Task OnPost()
    {
        await OnGet();
    }
}
