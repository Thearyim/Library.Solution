using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using Library.Models;

namespace Library.Controllers
{
  public class AuthorsController : Controller
  {

    [HttpGet("/authors")]
    public ActionResult Index()
    {
      List<Author> allAuthors = Author.GetAll();
      return View(allAuthors);
    }

    [HttpGet("/authors/new")]
    public ActionResult New()
    {
      return View();
    }

    [HttpPost("/authors")]
    public ActionResult Create(string authorName)
    {
      Author newAuthor = new Author(authorName);
      newAuthor.Save();
      List<Author> allAuthors = Author.GetAll();
      return View("Index", allAuthors);
    }

    [HttpGet("/authors/{id}")]
    public ActionResult Show(int id)
    {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Author selectedAuthor = Author.Find(id);
      List<Book> authorBooks = selectedAuthor.GetBooks();
      List<Book> allBooks = new List<Book>(Book.GetAll().Keys);
      model.Add("author", selectedAuthor);
      model.Add("authorBooks", authorBooks);
      model.Add("allBooks", allBooks);
      return View(model);
    }

    [HttpPost("/authors/{authorId}/books/new")]
    public ActionResult AddBook(int authorId, int bookId)
    {
      Author author = Author.Find(authorId);
      Book book = Book.Find(bookId);
      author.AddBook(book);
      return RedirectToAction("Show",  new { id = bookId });
    }

  }
}
