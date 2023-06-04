using dig.API.Common;

namespace dig.API.Feature.Auth;

public interface IAuthService
{
    public Task<Either<SimpleMessageError, UserDto>> Login(string email, string password);
    public Task<Either<SimpleMessageError, UserDto>> Register(string username, string email, string password);
}