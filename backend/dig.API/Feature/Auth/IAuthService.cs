using dig.API.Common;

namespace dig.API.Feature.Auth;

public interface IAuthService
{
    public Task<Either<SimpleMessageError, UserDto>> Login(string username, string password);
    public Task<Either<SimpleMessageError, UserDto>> Register(string username, string password);
}