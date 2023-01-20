namespace webPizzeria.Models
{
    public class Ingrediente
    {   public int Id { get; set; }
        public string Name { get; set; }

        public List<Pizza> Pizza { get; set; }
        public Ingrediente(){}
    }
}
