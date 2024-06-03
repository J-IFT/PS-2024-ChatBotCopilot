using System.ComponentModel.DataAnnotations;

namespace copilot_chatbot.Models
{
    public class GeneratedDataProduct
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created_at { get; set; }

        public int? ExportId { get; set; }
        public virtual Export Export { get; set; }

        public virtual ICollection<ProductKeyword> ProductKeywords { get; set; }
    }
}
