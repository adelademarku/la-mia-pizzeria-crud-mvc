using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webPizzeria.Database;
using Microsoft.SqlServer.Server;
using webPizzeria.Models;
using webPizzeria.Utilis;

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
                    .Include(pizza => pizza.Ingredienti)
                    .FirstOrDefault();


                if (pizzaTrovato != null)
                {
                    return View(pizzaTrovato);
                }

                return NotFound("La pizza con l'id cercato non esiste!");

            }

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using PizzaContext db = new PizzaContext();
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
                    return NotFound("La pizza da eliminare non è stata trovata!");
                }
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
                modelForView.Ingredienti = IngredientiConverter.getListTagsForMultipleSelect();


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

                    formData.Ingredienti = IngredientiConverter.getListTagsForMultipleSelect();
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
                Pizza pizzaToUpdate = db.Pizza.Where(pizza => pizza.Id == id).Include(pizza => pizza.Ingredienti).FirstOrDefault();

                if (pizzaToUpdate == null)
                {
                    return NotFound("La pizza non è stata strovata!");
                }
                else {

                    List<Category> categories = db.Categories.ToList<Category>();

                    PizzaCategoryView modelForView = new PizzaCategoryView();
                    modelForView.Pizza = pizzaToUpdate;
                    modelForView.Categories = categories;

                    List<Ingrediente> listIngredientiFromDb = db.Ingredienti.ToList<Ingrediente>();
                    List<SelectListItem> listaOpzioniPerLaSelect = new List<SelectListItem>();

                    foreach (Ingrediente ingrediente in listIngredientiFromDb)
                    {
                        bool eraStatoSelezionato = pizzaToUpdate.Ingredienti.Any(ingredienteSelezionato => ingredienteSelezionato.Id == ingrediente.Id);
                        SelectListItem opzioneSingolaSelect = new SelectListItem() { Text = ingrediente.Name, Value = ingrediente.Id.ToString(), Selected = eraStatoSelezionato };

                    }

                    modelForView.Ingredienti = listaOpzioniPerLaSelect;
                    return View("Update", modelForView);
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, PizzaCategoryView formData)
        {

            if (!ModelState.IsValid)
            {
                using (PizzaContext db = new PizzaContext())
                {
                    List<Category> categories = db.Categories.ToList<Category>();
                    formData.Categories = categories;
                }

                return View("Update", formData);
            }

            using (PizzaContext db = new PizzaContext())
            {
                Pizza pizzaToUpdate = db.Pizza.Where(pizza => pizza.Id == id).Include(pizza => pizza.Ingredienti).FirstOrDefault();

                if (pizzaToUpdate != null)
                {
                    pizzaToUpdate.Name = formData.Pizza.Name;
                    pizzaToUpdate.Description = formData.Pizza.Description;
                    pizzaToUpdate.Price = formData.Pizza.Price;
                    pizzaToUpdate.Image = formData.Pizza.Image;
                    pizzaToUpdate.Favorites = formData.Pizza.Favorites;
                    pizzaToUpdate.CategoryId = formData.Pizza.CategoryId;
                    //rimuovo i tag per inserirne i nuovi 
                    pizzaToUpdate.Ingredienti.Clear();

                    
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return NotFound("La pizza non è stata trovata!");
                }

            }

        }






















        }
    }

