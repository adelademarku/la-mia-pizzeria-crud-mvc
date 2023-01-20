using Microsoft.AspNetCore.Mvc.Rendering;
using webPizzeria.Database;
using webPizzeria.Models;

namespace webPizzeria.Utilis
{
    public class IngredientiConverter
    {

        public static List<SelectListItem> getListTagsForMultipleSelect()
        {
            using (PizzaContext db = new PizzaContext())
            {
                List<Ingrediente> ingredienteFromDb = db.Ingredienti.ToList<Ingrediente>();

                // Creare una lista di SelectListItem e tradurci al suo interno tutti i nostri Ingredienti che provengono da Db
                List<SelectListItem> ingredientiPerLaSelectMultipla = new List<SelectListItem>();

                foreach (Ingrediente ingrediente in ingredienteFromDb)
                {
                    SelectListItem opzioneSingolaSelectMultipla = new SelectListItem() { Text = ingrediente.Name, Value = ingrediente.Id.ToString() };
                    ingredientiPerLaSelectMultipla.Add(opzioneSingolaSelectMultipla);
                }

                return ingredientiPerLaSelectMultipla;
            }
        }
    }
}
