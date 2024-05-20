using System.ComponentModel.DataAnnotations;

namespace copilot_chatbot.Models
{
    public class Export
    {
        [Key]
        public int Id { get; set; }

        public bool IsProcessed { get; set; }

        public DateTime Exported_at { get; set; }

        // Foreign key for User
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int? ProductId { get; set; }

        // Navigation property pour GeneratedDataProducts
        public virtual ICollection<GeneratedDataProduct> GeneratedDataProducts { get; set; }
    }
}