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
        List<Book> allBooks = Book.GetAll();
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
      Book newBook = new Book(title, DateTime.Now.AddDays(30), false);
      newBook.Save();
      List<Book> allBooks = Book.GetAll();
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
