using DAL.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<UserDbEntity> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            // .UseNpgsql("Server=localhost;Port=5432;Database=monster;User Id=postgres;Password=123456;");
            .UseSqlite("DataSource=monster.sqlite");
        base.OnConfiguring(optionsBuilder);
    }
}