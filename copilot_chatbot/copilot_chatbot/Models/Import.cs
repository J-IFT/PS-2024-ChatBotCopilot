using System.ComponentModel.DataAnnotations;

namespace copilot_chatbot.Models
{
    public class Import
    {
        public int Id { get; set; }
        public DateTime Imported_at { get; set; }
        public bool IsProcessed { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

}