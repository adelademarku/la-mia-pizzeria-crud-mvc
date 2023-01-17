using Microsoft.AspNetCore.Mvc;
using webPizzeria.Database;
using webPizzeria.Models;

namespace webPizzeria.Controllers
{
    public class PizzeController : Controller
    {
        public IActionResult Index()
        {
            using (PizzaContext db = new PizzaContext())
            {
                List<Pizza> listaDellePizze = db.Pizza.ToList<Pizza>();
                return View(listaDellePizze);
            }

        }

        public IActionResult Detail(int id)
        {

            using (PizzaContext db = new PizzaContext())
            {
                // LINQ: syntax methos
                Pizza postTrovato = db.Pizza
                    .Where(SingoloPostNelDb => SingoloPostNelDb.Id == id)
                    .FirstOrDefault();


                if (postTrovato != null)
                {
                    return View(postTrovato);
                }

                return NotFound("Il post con l'id cercato non esiste!");

            }

        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pizza formData)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", formData);
            }

            using (PizzaContext db = new PizzaContext())
            {
                db.Pizza.Add(formData);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Update(int id)
        {
            using (PizzaContext db = new PizzaContext())
            {
                Pizza postToUpdate = db.Pizza.Where(pizza => pizza.Id == id).FirstOrDefault();

                if (postToUpdate == null)
                {
                    return NotFound("La pizza non è stata trovata");
                }

                return View("Update", postToUpdate);
            }

        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Pizza formPizza)
        {
            if (!ModelState.IsValid)
            {
                return View("Update", formPizza);
            }

            using (PizzaContext db = new PizzaContext())
            {
                Pizza postToUpdate = db.Pizza.Where(articolo => articolo.Id == formPizza.Id).FirstOrDefault();

                if (postToUpdate != null)
                {
                    postToUpdate.Name = formPizza.Name;
                    postToUpdate.Description = formPizza.Description;
                    postToUpdate.Image = formPizza.Image;
                    postToUpdate.Price = formPizza.Price;
                    postToUpdate.Favorites = formPizza.Favorites;

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    return NotFound("Il post che volevi modificare non è stato trovato!");
                }
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using (PizzaContext db = new PizzaContext())
            {
                Pizza pizzaToDelete = db.Pizza.Where(post => post.Id == id).FirstOrDefault();

                if (pizzaToDelete != null)
                {
                    db.Pizza.Remove(pizzaToDelete);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    return NotFound("Il post da eliminare non è stato trovato!");
                }
            }
        }
    }
}
