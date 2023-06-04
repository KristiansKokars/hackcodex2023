using dig.API.Feature.Auth;

public class SystemUser
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public UserAuthKey? AuthKey { get; set; }
}