namespace dig.API.Feature.Auth;

public struct Success { }

public struct Failure { }

public class Either<TError, TSuccess>
{
    private readonly TError _error;
    private readonly TSuccess _success;
    public readonly bool isError;

    public Either(TError error)
    {
        _error = error;
        isError = true;
    }

    public Either(TSuccess success)
    {
        _success = success;
        isError = false;
    }

    public T Map<T>(Func<TError, T> error, Func<TSuccess, T> success)
    {
        return isError ? error(_error) : success(_success);
    }
    
    public async Task<T> MapAsync<T>(Func<TError, Task<T>> error, Func<TSuccess, Task<T>> success)
    {
        return isError ? await error(_error) : await success(_success);
    }

    public void OnSuccess<T>(Func<TSuccess, T> success)
    {
        if (!isError) success(_success);
    }

    public void OnError<T>(Func<TError, T> error)
    {
        if (isError) error(_error);
    }
}