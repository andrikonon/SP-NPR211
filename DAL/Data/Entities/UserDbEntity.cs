using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Data.Entities;

[Table("tblUsers")]
public class UserDbEntity
{
    [Key]
    public Guid Id { get; set; }
    [Required, StringLength(200)]
    public string FirstName { get; set; }
    [Required, StringLength(200)]
    public string LastName { get; set; }
    [Required, StringLength(200), EmailAddress]
    public string Email { get; set; }
    [Required]
    public int PasswordHash { get; set; }
    [Required]
    public DateTime DateCreated { get; set; }
}