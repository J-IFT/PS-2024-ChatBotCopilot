using System.ComponentModel.DataAnnotations;

namespace copilot_chatbot.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } 

        [Required]
        public string Species { get; set; }

        public string? Type { get; set; }

        public string? Size { get; set; }

        public string? Blooming_season { get; set; }

        public string? Color { get; set; }

        public string Exposition{ get; set; }

        [Required]
        public DateTime Last_updated { get; set; }

        public virtual ICollection<Import> Imports { get; set; }
    }
}
