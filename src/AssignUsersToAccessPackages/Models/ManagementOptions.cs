namespace AssignUsersToAccessPackages.Models;

public class ManagementOptions
{
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
}
