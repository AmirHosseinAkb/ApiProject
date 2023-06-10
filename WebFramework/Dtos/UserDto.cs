using System.ComponentModel.DataAnnotations;
using Entities.User;

namespace WebFramework.Dtos;

public class UserDto:IValidatableObject
{
    [Required]
    [MaxLength(200)]
    public string UserName { get; set; }
    [Required]
    [MaxLength(50)]
    public string Password { get; set; }
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; }
    public int Age { get; set; }
    public GenderType Gender { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (UserName.Equals("test", StringComparison.OrdinalIgnoreCase))
            yield return new ValidationResult("User Name Cannot Be test");

        if (Password.Equals("12345678"))
            yield return new ValidationResult("Password Pattern is very weak please choose any else");
    }
}