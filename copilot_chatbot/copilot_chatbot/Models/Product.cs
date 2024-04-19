namespace copilot_chatbot.Models
{
    // Product.cs
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Features { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
    }

    // ResponseModel.cs
    public class ResponseModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
    }
}
