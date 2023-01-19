using Microsoft.AspNetCore.Mvc.Rendering;

namespace webPizzeria.Models
{
    public class PizzaCategoryView
    {
        public Pizza Pizza { get; set; }

        public List<Category> ? Categories { get; set;}

        public List<SelectListItem>? Ingredienti { get; set; }

        public List<SelectListItem>? IngredientiSelectedFromMultipleSelect { get; set; }
    }
}
