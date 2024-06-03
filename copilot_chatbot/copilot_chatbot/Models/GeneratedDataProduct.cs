﻿using System;
using System.Collections.Generic;

namespace copilot_chatbot.Models
{
    public class GeneratedDataProduct
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created_at { get; set; }

        // Propriété pour la relation avec le produit
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        // Propriété pour la relation avec Export
        public int? ExportId { get; set; }
        public virtual Export Export { get; set; }

        // Propriété pour la relation avec ProductKeyword
        public virtual ICollection<ProductKeyword> ProductKeywords { get; set; }
    }
}
