using EntraDemo.Models;
using EntraDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

namespace EntraDemo.Pages;

[AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
public class ManageUserModel(ILogger<ManageUserModel> logger, GraphServiceClient graphServiceClient, ManagementService managementService, IOptions<UserOptions> options) : PageModel
{
    private readonly GraphServiceClient _graphServiceClient = graphServiceClient;
    private readonly ManagementService _managementService = managementService;
    private readonly ILogger<ManageUserModel> _logger = logger;

    public bool IsNewUser { get; set; } = true;
    public string NewUserPassword { get; set; } = string.Empty;

    [BindProperty]
    public UserModel User { get; set; } = new();

    public async Task OnGet(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            User.Department = options.Value.DefaultDepartment;
        }
        else
        {
            User = await _managementService.GetUser(id);
        }

        IsNewUser = string.IsNullOrEmpty(User.ID);
    }

    public async Task OnPost(string action)
    {
        if (action == "save")
        {
            var password = await _managementService.ManageUser(User);
            if (string.IsNullOrEmpty(password))
            {
                Response.Redirect("/");
            }
            else
            {
                NewUserPassword = password;
            }
        }
        else if (action == "remove")
        {
            await _managementService.RemoveUser(User.ID);
            Response.Redirect("/");
        }
    }
}
