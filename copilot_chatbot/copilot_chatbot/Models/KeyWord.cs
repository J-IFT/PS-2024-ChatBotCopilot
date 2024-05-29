using System.ComponentModel.DataAnnotations;

namespace copilot_chatbot.Models
{
    public class Keyword
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<ProductKeyword> ProductKeywords { get; set; }
    }
}
