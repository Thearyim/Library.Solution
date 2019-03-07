using Microsoft.AspNetCore.Mvc;
using Library.Models;
using System.Collections.Generic;
using System;

namespace Library.Controllers
{
    public class BooksController : Controller
    {

        [HttpGet("/books")]
        public ActionResult Index()
        {
            Dictionary<Book, int> allBooks = Book.GetAll();
            return View(allBooks);
        }

        [HttpGet("/books/new")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost("/books")]
        public ActionResult Create(string title)
        {
            Book newBook = new Book(title);
            newBook.Save();

            // The GetAll() method returns a Dictionary<Book, int>
            // where the Key is a 'Book' and the Value is the number of copies 
            // of that book the library has.
            List<Book> allBooks = new List<Book>(Book.GetAll().Keys);
            return View("Index", allBooks);
        }

        // [HttpGet("/items/{id}")]
        // public ActionResult Show(int id)
        // {
        //   Dictionary<string, object> model = new Dictionary<string, object>();
        //   return View(model);
        // }

        [HttpPost("/books/delete")]
        public ActionResult DeleteAll()
        {
            Book.ClearAll();
            return View();
        }
    }
}
