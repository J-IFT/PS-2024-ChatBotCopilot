using System.ComponentModel.DataAnnotations;

namespace copilot_chatbot.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        public virtual ICollection<Import> Imports { get; set; }
        public virtual ICollection<Export> Exports { get; set; }
    }
}