using System.ComponentModel.DataAnnotations;

namespace NewTaskApp.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
