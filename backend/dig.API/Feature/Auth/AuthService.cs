using dig.API.Common;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace dig.API.Feature.Auth;

public class AuthService : IAuthService
{
    private readonly DigitexDb _db;

    public AuthService(DigitexDb db)
    {
        _db = db;
    }

    public async Task<Either<SimpleMessageError, UserDto>> Login(string username, string password)
    {
        var user = await _db.SystemUsers
            .Where(user => user.Username == username)
            .Include(user => user.AuthKey)
            .SingleOrDefaultAsync();

        if (user is null)
        {
            // TODO: we would usually have error codes, but for now, we will pass simple string messages back
            return new Either<SimpleMessageError, UserDto>(new SimpleMessageError("No such user exists!"));
        }

        var isPasswordCorrect = BC.Verify(password, user.AuthKey.Password);

        if (!isPasswordCorrect)
        {
            return new Either<SimpleMessageError, UserDto>(new SimpleMessageError("Incorrect password!"));
        }

        return new Either<SimpleMessageError, UserDto>(new UserDto(user.Id.ToString(), user.Username));
    }

    public async Task<Either<SimpleMessageError, UserDto>> Register(string username, string password)
    {
        // TODO: temporary manual check, usually our database would take care of this
        var existingUser = await _db.SystemUsers
            .Where(user => user.Username == username)
            .SingleOrDefaultAsync();

        if (existingUser is not null)
        {
            return new Either<SimpleMessageError, UserDto>(new SimpleMessageError("User already exists with this name!"));
        }
        
        var hashedPassword = BC.HashPassword(password);
        var userId = Guid.NewGuid();
        var authKey = new UserAuthKey
        {
            Id = userId,
            Password = hashedPassword
        };
        var systemUser = new SystemUser
        {
            Id = userId,
            Username = username,
            AuthKey = authKey
        };

        // TODO: temporary error handling to catch all with a simple error to pass exception along, not fit for prod
        try
        {
            await _db.SystemUsers.AddAsync(systemUser);
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new Either<SimpleMessageError, UserDto>(
                new SimpleMessageError($"Failed to add the user to the database! Error: {ex.Message}"));
        }
        
        return new Either<SimpleMessageError, UserDto>(new UserDto(userId.ToString(), systemUser.Username));
    }
}