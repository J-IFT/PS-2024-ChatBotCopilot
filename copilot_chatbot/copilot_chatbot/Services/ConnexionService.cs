using System.Linq;
using copilot_chatbot.Models;

namespace copilot_chatbot.Services
{
    public class ConnexionService
    {
        private readonly ApplicationDbContext _context;

        public ConnexionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsUserValid(string username, string email, string password)
        {
            // Recherche de l'utilisateur par nom d'utilisateur ou e-mail
            var user = _context.Users.FirstOrDefault(u => u.Username == username || u.Email == email);

            if (user != null)
            {
                // Vérification du mot de passe
                return user.Password == password;
            }

            // Aucun utilisateur trouvé avec les informations fournies
            return false;
        }
    }
}
