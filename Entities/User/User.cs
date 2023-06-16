using System.ComponentModel.DataAnnotations;

namespace Entities.User
{
    public class User : BaseEntity
    {
        public User()
        {
            IsActive = true;
<<<<<<< Updated upstream
            LastLoginDate= DateTime.Now;
=======
            SecurityStamp = Guid.NewGuid();
>>>>>>> Stashed changes
        }
        
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }
        [Required]
        [StringLength(500)]
        public string PasswordHash { get; set; }
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        public int Age { get; set; }
        public GenderType Gender { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        public Guid SecurityStamp { get; set; }


        public ICollection<Post.Post> Posts { get; set; }
    }

    public enum GenderType
    {
        [Display(Name = "مرد")]
        Male = 1,

        [Display(Name = "زن")]
        Female = 2
    }
}
