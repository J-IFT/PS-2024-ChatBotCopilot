using System.ComponentModel.DataAnnotations;

namespace copilot_chatbot.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public DateTime Last_updated { get; set; }
        public string Blooming_season { get; set; }
        public string Color { get; set; }
        public string Exposition { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }

        public ICollection<Import> Imports { get; set; }
    }
}
