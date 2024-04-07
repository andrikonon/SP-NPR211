using Bogus;
using DAL.Data;
using DAL.Data.Entities;
using DAL.Events;
using Microsoft.EntityFrameworkCore;

namespace DAL.Services;

public class UserService
{
    private readonly ApplicationDbContext _context;
    public event UserEvents.UserInsertItemDelegate InsertUserEvent;

    public UserService()
    {
        _context = new ApplicationDbContext();
        _context.Database.Migrate();
    }

    public void InsertRandomUser(int count)
    {
        var faker = new Faker<UserDbEntity>("uk")
            .RuleFor(user => user.Id, _ => Guid.NewGuid())
            .RuleFor(user => user.FirstName, f => f.Person.FirstName)
            .RuleFor(user => user.LastName, f => f.Person.LastName)
            .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.FirstName, user.LastName))
            .RuleFor(user => user.PasswordHash, f => f.Internet.Password().GetHashCode());

        var users = faker.Generate(count);
        int i = 0;
        foreach (var user in users)
        {
            user.DateCreated = DateTime.UtcNow;
            _context.Users.Add(user);
            _context.SaveChanges();
            InsertUserEvent.Invoke(++i);
        }
    }

    public void DeleteLastUsers(int count)
    {
        for (var _ = 0; _ < count; _++)
        {
            var last = _context.Users.OrderBy(user => user.DateCreated).Last();
            _context.Users.Remove(last);
            _context.SaveChanges();
        }
        
    }
}