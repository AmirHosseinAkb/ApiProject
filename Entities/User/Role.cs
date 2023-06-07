using System.ComponentModel.DataAnnotations;

namespace Entities.User
{
    public class Role : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }
    }
}
