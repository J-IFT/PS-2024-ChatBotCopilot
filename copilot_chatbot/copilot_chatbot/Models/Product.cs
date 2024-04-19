using System.ComponentModel.DataAnnotations;

namespace copilot_chatbot.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Characteristics { get; set; } // JSON format, including name, and other product characteristics

        [Required]
        public string GeneratedData { get; set; } // JSON format, including title, description, and keywords


        public DateTime Last_updated { get; set; }

        // Navigation property for Imports (optional)
        public virtual ICollection<Import> Imports { get; set; }
    }
}
