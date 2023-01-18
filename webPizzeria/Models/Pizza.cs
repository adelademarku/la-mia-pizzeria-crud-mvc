using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;



namespace webPizzeria.Models
{
    public class Pizza
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(100)")]
        [StringLength(100, ErrorMessage = "Il campo titolo non può contenere più di 100 caratteri")]
        public string Name { get; set; }

        [Column(TypeName = "text")]
        public string Description { get; set; }

        [Column(TypeName = "varchar(300)")]
        [StringLength(300, ErrorMessage = "Il campo titolo non può contenere più di 300 caratteri")]
        public string Image { get; set; }


        public double Price { get; set; }


        public bool Favorites { get; set; }

        public int? CategoryId { get; set; }
        public Category?  Category { get; set; } 

        public Pizza() { }

        public Pizza(string name, string description, string image, double price, bool favorites)
        {
            this.Name = name;
            this.Description = description;
            this.Image = image;
            this.Price = price;
            this.Favorites = favorites;

        }
    }
}