namespace EntraDemo.Models;

public class ManagementOptions
{
    public string Domain { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string UserQuery { get; set; } = string.Empty;
    public string AccessQuery { get; set; } = string.Empty;
}
