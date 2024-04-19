using System.ComponentModel.DataAnnotations;

namespace copilot_chatbot.Models
{
    public class Import
    {
        [Key]
        public int Id { get; set; }

        public DateTime ImportedAt { get; set; }

        public bool Status { get; set; }

        // Foreign key for User
        public int UserId { get; set; }
        public virtual User User { get; set; }

        // Foreign key for Product
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
