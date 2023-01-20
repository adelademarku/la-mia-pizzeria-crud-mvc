using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using webPizzeria.Models;

namespace webPizzeria.Database
{
    public class PizzaContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Pizza> Pizza { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Ingrediente> Ingredienti { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost; Database=PizzeriaWeb;" + "Integrated Security=True;TrustServerCertificate=True");
        }
    }
}
