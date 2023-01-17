using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.SqlServer.Server;
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
                Pizza pizzaTrovato = db.Pizza
                    .Where(SingoloPizzaNelDb => SingoloPizzaNelDb.Id == id)
                    .Include(pizza => pizza.Category)
                    .FirstOrDefault();


                if (pizzaTrovato != null)
                {
                    return View(pizzaTrovato);
                }

                return NotFound("Il post con l'id cercato non esiste!");

            }

        }

        [HttpGet]
        public IActionResult Create()
        {
            using (PizzaContext db = new PizzaContext())
            {
                List<Category> categoriesFromDb = db.Categories.ToList<Category>();

                PizzaCategoryView modelForView = new PizzaCategoryView();
                modelForView.Pizza = new Pizza();

                modelForView.Categories = categoriesFromDb;

                return View("Create", modelForView);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PizzaCategoryView formData)
        {
            if (!ModelState.IsValid)
            {
                using (PizzaContext db = new PizzaContext())
                {
                    List<Category> categories = db.Categories.ToList<Category>();

                    formData.Categories = categories;
                }


                return View("Create", formData);
            }

            using (PizzaContext db = new PizzaContext())
            {
                db.Pizza.Add(formData.Pizza);
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

                List<Category> categories = db.Categories.ToList<Category>();

                PizzaCategoryView modelForView = new PizzaCategoryView();
                modelForView.Pizza = postToUpdate;
                modelForView.Categories = categories;

                return View("Update", modelForView);
            }

        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, PizzaCategoryView formPizza)
        {
            if (!ModelState.IsValid)
            {

                using (PizzaContext db = new PizzaContext())
                {
                    List<Category> categories = db.Categories.ToList<Category>();

                    formPizza.Categories = categories;
                }
                return View("Update", formPizza);
            }

            using (PizzaContext db = new PizzaContext())
            {
                Pizza postToUpdate = db.Pizza.Where(pizza => pizza.Id == id).FirstOrDefault();

                if (postToUpdate != null)
                {
                    postToUpdate.Name = formPizza.Pizza.Name;
                    postToUpdate.Description = formPizza.Pizza.Description;
                    postToUpdate.Image = formPizza.Pizza.Image;
                    postToUpdate.Price = formPizza.Pizza.Price;
                    postToUpdate.Favorites = formPizza.Pizza.Favorites;
                    postToUpdate.CategoryId = formPizza.Pizza.CategoryId;

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
                Pizza pizzaToDelete = db.Pizza.Where(pizza => pizza.Id == id).FirstOrDefault();

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
