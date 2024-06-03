using System.Collections.Generic;

namespace copilot_chatbot.Utilities
{
    public class ExcelManager
    {
        public List<Product> ReadExcel()
        {
            // À compléter : code pour lire un fichier Excel et retourner une liste de produits
            return new List<Product>();
        }

        public void WriteExcel(List<Product> products)
        {
            // À compléter : code pour écrire une liste de produits dans un fichier Excel
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Features { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
    }
}
