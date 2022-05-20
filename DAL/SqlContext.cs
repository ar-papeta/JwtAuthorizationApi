using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class SqlContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public SqlContext(DbContextOptions<SqlContext> options)
            : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.EMail).IsUnique();

        var initialUserAdmin = new User
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            EMail = "artem@gmail.com",
            Name = "Artem",
            Role = RoleNames.Admin,
            Password = "Owve1iNLlEKKrO3hQplQLBNN3TfIkzMEXwF8EkikVN4="
        };

        modelBuilder.Entity<User>().HasData(initialUserAdmin);
    }
}
