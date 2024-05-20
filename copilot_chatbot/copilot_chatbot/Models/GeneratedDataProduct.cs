using System.ComponentModel.DataAnnotations;

namespace copilot_chatbot.Models
{
    public class GeneratedDataProduct
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime Created_at { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public virtual ICollection<ProductKeyword> ProductKeyword { get; set; }
        public virtual ICollection<Export> Exports { get; set; }
    }
}
