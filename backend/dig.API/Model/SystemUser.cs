using dig.API.Feature.Auth;

public class SystemUser
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    
    public UserAuthKey? AuthKey { get; set; }
}