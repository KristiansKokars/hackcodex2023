public class Helpers
{
    public static long ConvertToUnixMillis(DateTime date)
    {
        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        long unixMilliseconds = (long)(date.ToUniversalTime() - unixEpoch).TotalMilliseconds;
        return unixMilliseconds;
    }
}