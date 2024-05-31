using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace copilot_chatbot.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public ICollection<Import> Imports { get; set; }
        public ICollection<Export> Exports { get; set; } 
    }
}
