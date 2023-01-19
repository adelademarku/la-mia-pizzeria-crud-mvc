using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.SqlServer.Server;
using webPizzeria.Database;
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

        [HttpGet]
        public IActionResult Create()
        {
            using (PizzaContext db = new PizzaContext())
            {
                List<Category> categoriesFromDb = db.Categories.ToList<Category>();

                PizzaCategoryView modelForView = new PizzaCategoryView();
                modelForView.Pizza = new Pizza();

                modelForView.Categories = categoriesFromDb;
                modelForView.Ingredienti= IngredientiConverter.getListTagsForMultipleSelect();
               

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

                if (formData.IngredientiSelectedFromMultipleSelect != null)
                {
                    formData.Pizza.Ingredienti = new List<Ingrediente>();

                    System.Collections.IList list = formData.IngredientiSelectedFromMultipleSelect;
                    for (int i = 0; i < list.Count; i++)
                    {
                        string ingredienteId = (string)list[i];
                        int ingredienteIdIntFromSelect = int.Parse(ingredienteId);

                        Ingrediente tag = db.Ingredienti.Where(ingredienteDb => ingredienteDb.Id == ingredienteIdIntFromSelect).FirstOrDefault();

                        // todo controllare eventuali altri errori tipo l'id del tag non esiste
                        formData.Pizza.Ingredienti.Add(tag);
                    }
                }

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
                Pizza pizzaToUpdate = db.Pizza.Where(tipidipizza => tipidipizza.Id == id).Include(pizza => pizza.Ingredienti).FirstOrDefault();

                if (pizzaToUpdate == null)
                {
                    return NotFound("La pizza non è stata trovata");
                }

                List<Category> categories = db.Categories.ToList<Category>();

                PizzaCategoryView modelForView = new PizzaCategoryView();
                modelForView.Pizza = pizzaToUpdate;
                modelForView.Categories = categories;


                List<Ingrediente> listIngredientiFromDb = db.Ingredienti.ToList<Ingrediente>();

                List<SelectListItem> listaOpzioniPerLaSelect = new List<SelectListItem>();

                foreach (Ingrediente ingrediente in listIngredientiFromDb)
                {
                    // Ricerco se il tag che sto inserindo nella lista delle opzioni della select era già stato selezionato dall'utente
                    // all'interno della lista dei tag del post da modificare
                    bool eraStatoSelezionato = pizzaToUpdate.Ingredienti.Any(ingredienteSelezionati => ingredienteSelezionati.Id == ingrediente.Id);

                    SelectListItem opzioneSingolaSelect = new SelectListItem() { Text = ingrediente.Name, Value = ingrediente.Id.ToString(), Selected = eraStatoSelezionato };
                    listaOpzioniPerLaSelect.Add(opzioneSingolaSelect);
                }

                modelForView.Ingredienti = listaOpzioniPerLaSelect;

                return View("Update", modelForView);
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
                Pizza postToUpdate = db.Pizza.Where(tipidipizza => tipidipizza.Id == id).Include(pizza => pizza.Ingredienti).FirstOrDefault();

                if (postToUpdate != null)
                {
                    postToUpdate.Name = formData.Pizza.Name;
                    postToUpdate.Description = formData.Pizza.Description;
                    postToUpdate.Image = formData.Pizza.Image;
                    postToUpdate.Price = formData.Pizza.Price;
                    postToUpdate.Favorites = formData.Pizza.Favorites;
                    postToUpdate.CategoryId = formData.Pizza.CategoryId;

                    // rimuoviamo i tag e inseriamo i nuovi
                    postToUpdate.Ingredienti.Clear();


                    if (formData.IngredientiSelectedFromMultipleSelect != null)
                    {

                        System.Collections.IList ingredienti = formData.IngredientiSelectedFromMultipleSelect;
                        for (int i = 0; i < ingredienti.Count; i++)
                        {
                            string ingredientiId = (string)ingredienti[i];
                            int ingredientiIdIntFromSelect = int.Parse(ingredientiId);

                            Ingrediente ingrediente = db.Ingredienti.Where(ingredienteDb => ingredienteDb.Id == ingredientiIdIntFromSelect).FirstOrDefault();

                            // todo controllare eventuali altri errori tipo l'id del tag non esiste

                            postToUpdate.Ingredienti.Add(ingrediente);
                        }
                    }

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
