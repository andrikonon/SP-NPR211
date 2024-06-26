using Bogus;
using DAL.Data;
using DAL.Data.Entities;
using DAL.Events;
using Dapper;
using System.Data.SQLite;
using Microsoft.EntityFrameworkCore;

namespace DAL.Services;

public class UserService
{
    private readonly ApplicationDbContext _context;
    public event UserEvents.UserInsertItemDelegate InsertUserEvent;
    private CancellationToken _cancellationToken;

    public UserService()
    {
        _context = new ApplicationDbContext();
        _context.Database.Migrate();
    }

    public UserService(CancellationToken token) : this()
    {
        _cancellationToken = token;
    }

    public void InsertRandomUser(int count)
    {
        var faker = new Faker<UserDbEntity>("uk")
            // .RuleFor(user => user.Id, _ => Guid.NewGuid())
            .RuleFor(user => user.FirstName, f => f.Person.FirstName)
            .RuleFor(user => user.LastName, f => f.Person.LastName)
            .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.FirstName, user.LastName))
            .RuleFor(user => user.PasswordHash, f => f.Internet.Password().GetHashCode());

        var users = faker.Generate(count);
        int i = 0;
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                foreach (var user in users)
                {
                    user.DateCreated = DateTime.UtcNow;
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    InsertUserEvent(++i);

                    if (_cancellationToken.IsCancellationRequested)
                    {
                        throw new Exception("Cansel operation");
                    }
                }

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback(); // Rollback the transaction if an exception occurs
                InsertRandomUser(0); //операція була скасована
            }
        }
    }
    
    public Task InsertRandomUserAsync(int count)
    {
        return Task.Run(() => InsertRandomUser(count));
    }

    public void InsertRandomUserDapper(int count)
    {
        var faker = new Faker<UserDbEntity>("uk")
            // .RuleFor(user => user.Id, _ => Guid.NewGuid())
            .RuleFor(user => user.FirstName, f => f.Person.FirstName)
            .RuleFor(user => user.LastName, f => f.Person.LastName)
            .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.FirstName, user.LastName))
            .RuleFor(user => user.PasswordHash, f => f.Internet.Password().GetHashCode());

        var users = faker.Generate(count);
        int i = 0;
        using var conn = new SQLiteConnection(Constants.Database.ConnectionString);
        foreach (var user in users)
        {
            string query = "INSERT INTO tblUsers (LastName, FirstName, Email, PasswordHash, DateCreated) VALUES " +
                           "(@LastName, @FirstName, @Email, @PasswordHash, @DateCreated)";
            conn.Execute(query, user);
            InsertUserEvent(++i);
        }
    }

    public Task InsertRandomUserDapperAsync(int count)
    {
        return Task.Run(() => InsertRandomUserDapper(count));
    }

    public IEnumerable<UserDbEntity> GetUsers()
    {
        using var conn = new SQLiteConnection(Constants.Database.ConnectionString);
        string query = "SELECT Id, LastName, FirstName, Email, PasswordHash, DateCreated FROM tblUsers;";
        return conn.Query<UserDbEntity>(query);
    }
}