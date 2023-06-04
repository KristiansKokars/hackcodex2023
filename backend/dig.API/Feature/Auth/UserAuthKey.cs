using System.ComponentModel.DataAnnotations;

namespace dig.API.Feature.Auth;

public class UserAuthKey
{
    [Key]
    public Guid Id { set; get; }
    public string Password { set; get; }
}