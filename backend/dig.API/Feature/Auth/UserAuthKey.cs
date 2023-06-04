using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dig.API.Feature.Auth;

public class UserAuthKey
{
    [Key]
    [ForeignKey("User")]
    public Guid Id { set; get; }

    public virtual SystemUser User { get; set; }
    [Required]
    public string Password { set; get; }
}