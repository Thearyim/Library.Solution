using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;
namespace Library.Models
{
  public class Book
  {
    private bool _checkedOut;
    private string _title;
    private DateTime _due_date;
    private int _id;
    public Book(string title, int id = 0)
    {
      _title = title;
      _due_date = DateTime.Now.AddDays(30);
      _id = id;
    }

    public Book(string title, DateTime due_date, bool checkedOut, int id = 0)
    {
      _title = title;
      _id = id;
      _checkedOut = checkedOut;
      _due_date = due_date;
    }

    public static void ClearAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM books;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static Book Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM books WHERE id = (@searchId);";
      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int bookId = 0;
      string bookTitle = "";
      while(rdr.Read())
      {
        bookId = rdr.GetInt32(0);
        bookTitle = rdr.GetString(1);
      }
      Book newBook = new Book(bookTitle, bookId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newBook;
    }

    public static List<Book> GetAll()
    {
      List<Book> allBooks = new List<Book> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM books;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int bookId = rdr.GetInt32(0);
        string bookTitle = rdr.GetString(1);
        Book newBook = new Book(bookTitle, bookId);
        allBooks.Add(newBook);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allBooks;
    }

    public void AddAuthor(Author newAuthor)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO copies (author_id, book_id) VALUES (@AuthorId, @BookId);";
      MySqlParameter author_id = new MySqlParameter();
      author_id.ParameterName = "@AuthorId";
      author_id.Value = newAuthor.GetId();
      cmd.Parameters.Add(author_id);
      MySqlParameter book_id = new MySqlParameter();
      book_id.ParameterName = "@BookId";
      book_id.Value = _id;
      cmd.Parameters.Add(book_id);
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM books WHERE id = @BookId; DELETE FROM copies WHERE book_id = @BookId;";
      MySqlParameter bookIdParameter = new MySqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);
      cmd.ExecuteNonQuery();
      if (conn != null)
      {
        conn.Close();
      }
    }

    public void Edit(string newTitle, DateTime newDueDate)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE books SET description = @newTitle, due_date = @dueDate WHERE id = @searchId;";
      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);
      MySqlParameter title = new MySqlParameter();
      title.ParameterName = "@newTitle";
      title.Value = newTitle;
      cmd.Parameters.Add(title);
      MySqlParameter dueDate = new MySqlParameter();
      dueDate.ParameterName = "@dueDate";
      dueDate.Value = newDueDate;
      cmd.ExecuteNonQuery();
      _title = newTitle;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public override bool Equals(System.Object otherBook)
    {
      if (!(otherBook is Book))
      {
        return false;
      }
      else
      {
        Book newBook = (Book)otherBook;
        bool idEquality = (this.GetId() == newBook.GetId());
        bool titleEquality = (this.GetTitle() == newBook.GetTitle());
        bool dueDateEquality = (this.GetDueDate() == newBook.GetDueDate());
        return (idEquality && titleEquality && dueDateEquality);
      }
    }

    public List<Author> GetAuthors()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT authors.* FROM books
          JOIN authors_books ON (books.id = authors_books.book_id)
          JOIN authors ON (authors_books.author_id = authors.id)
          WHERE books.id = @BookId;";
      MySqlParameter bookIdParameter = new MySqlParameter();
      bookIdParameter.ParameterName = "@bookId";
      bookIdParameter.Value = _id;
      cmd.Parameters.Add(bookIdParameter);
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      List<Author> authors = new List<Author> {};
      while (rdr.Read())
      {
          int thisAuthorId = rdr.GetInt32(0);
          string authorName = rdr.GetString(1);
          Author foundAuthor = new Author(authorName, thisAuthorId);
          authors.Add(foundAuthor);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return authors;
    }

    public bool GetCheckedOut()
    {
      return _checkedOut;
    }

    public string GetTitle()
    {
      return _title;
    }

    public DateTime GetDueDate()
    {
        return _due_date;
    }

    public override int GetHashCode()
    {
        return this.GetId().GetHashCode();
    }

    public int GetId()
    {
      return _id;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO books (title) VALUES (@title);";
      MySqlParameter title = new MySqlParameter();
      title.ParameterName = "@title";
      title.Value = this._title;
      cmd.Parameters.Add(title);
      cmd.ExecuteNonQuery();
      _id = (int)cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
    }

    public void SetTitle(string newTitle)
    {
      _title = newTitle;
    }
  }
}
