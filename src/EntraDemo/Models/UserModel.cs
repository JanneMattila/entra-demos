namespace EntraDemo.Models;

public class UserModel
{
    public string ID { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string UserPrincipalName { get; set; } = string.Empty;
    public bool AccountEnabled { get; set; } = true;
}
