using dig.API.Feature.Auth;
using Microsoft.EntityFrameworkCore;

public class DigitexDb : DbContext
{
    public DigitexDb(DbContextOptions<DigitexDb> options)
        : base(options) { }

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<SystemUser> SystemUsers => Set<SystemUser>();
    public DbSet<UserAuthKey> UserAuthKeys => Set<UserAuthKey>();
}