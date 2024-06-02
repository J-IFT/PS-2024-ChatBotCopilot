using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace copilot_chatbot.Models
{
    [Index(nameof(GeneratedDataProductId), nameof(KeywordId), IsUnique = true)]
    public class ProductKeyword
    {
        [Key]
        public int Id { get; set; }

        // Foreign key pour GeneratedDataProduct
        public int GeneratedDataProductId { get; set; }
        public virtual GeneratedDataProduct GeneratedDataProduct { get; set; }

        // Foreign key pour Keyword
        public int KeywordId { get; set; }
        public virtual Keyword Keyword { get; set; }
    }
}