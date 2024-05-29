using System.ComponentModel.DataAnnotations;

namespace copilot_chatbot.Models
{
    public class Import
    {
        [Key]
        public int Id { get; set; }

        public bool IsProcessed { get; set; }

        public DateTime Imported_at { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}