using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace Library.Models
{
  public class Author
  {
    private string _name;
    private int _id;

    public Author(string authorName, int id = 0)
    {
      _name = authorName;
      _id = id;
    }

    public string GetName()
    {
      return _name;
    }

    public int GetId()
    {
      return _id;
    }

    public static void ClearAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM copies; DELETE FROM author;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static List<Author> GetAll()
    {
      List<Author> allAuthors = new List<Author> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM author;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int AuthorId = rdr.GetInt32(0);
        string AuthorName = rdr.GetString(1);
        Author newAuthor = new Author(AuthorName, AuthorId);
        allAuthors.Add(newAuthor);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allAuthors;
    }

    public static Author Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM author WHERE id = (@searchId);";
      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int AuthorId = 0;
      string AuthorName = "";
      while(rdr.Read())
      {
        AuthorId = rdr.GetInt32(0);
        AuthorName = rdr.GetString(1);
      }
      Author newAuthor = new Author(AuthorName, AuthorId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newAuthor;
    }

    public List<Book> GetBooks(string sortBy = "")
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      if (sortBy == "")
      {
        cmd.CommandText = @"SELECT books.*, copies.due_date, copies.is_checkedOut FROM author, copies.is_checkedOut FROM author JOIN copies ON (author.id = copies.author_id) JOIN books ON (copies.book_id = books.id) WHERE author.id = @AuthorId;";
      }
      else {
        cmd.CommandText = @"SELECT books.*, copies.due_date, copies.is_checkedOut FROM authors JOIN copies ON (author.id = copies.author_id) JOIN books ON (copies.book_id = books.id) WHERE author.id = @AuthorId ORDER BY books." + sortBy + ";";
      }
      MySqlParameter authorIdParameter = new MySqlParameter();
      authorIdParameter.ParameterName = "@AuthorId";
      authorIdParameter.Value = _id;
      cmd.Parameters.Add(authorIdParameter);
      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      List<Book> books = new List<Book>{};
      while(rdr.Read())
      {
        int thisBookId = rdr.GetInt32(0);
        string bookTitle = rdr.GetString(1);
        DateTime bookDueDate = rdr.GetDateTime(2);
        bool isCheckedOut = rdr.GetBoolean(3);
        Book foundBook = new Book(bookTitle, bookDueDate, id: thisBookId, checkedOut: isCheckedOut);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return books;
    }

    public override bool Equals(System.Object otherAuthor)
    {
      if (!(otherAuthor is Author))
      {
        return false;
      }
      else
      {
        Author newAuthor = (Author) otherAuthor;
        bool idEquality = this.GetId().Equals(newAuthor.GetId());
        bool nameEquality = this.GetName().Equals(newAuthor.GetName());
        return (idEquality && nameEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetId().GetHashCode();
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO author (name) VALUES (@name);";
      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@name";
      name.Value = this._name;
      cmd.Parameters.Add(name);
      cmd.ExecuteNonQuery();
      _id = (int)cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void SaveBook(Book book)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO copies (author_id, book_id, due_date) VALUES (@authorId, @bookId, @dueDate);";
      MySqlParameter authorId = new MySqlParameter();
      authorId.ParameterName = "@authorId";
      authorId.Value = this._id;
      MySqlParameter bookId = new MySqlParameter();
      bookId.ParameterName = "@bookId";
      bookId.Value = book.GetId();
      MySqlParameter dueDate = new MySqlParameter();
      dueDate.ParameterName = "@dueDate";
      dueDate.Value = book.GetDueDate();
      cmd.Parameters.Add(authorId);
      cmd.Parameters.Add(bookId);
      cmd.Parameters.Add(dueDate);
      cmd.ExecuteNonQuery();
      _id = (int)cmd.LastInsertedId;
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
      MySqlCommand cmd = new MySqlCommand("DELETE FROM copies WHERE author_id = @AuthorId; DELETE FROM author WHERE id = @AuthorId;", conn);
      MySqlParameter authorIdParameter = new MySqlParameter();
      authorIdParameter.ParameterName = "@AuthorId";
      authorIdParameter.Value = this.GetId();
      cmd.Parameters.Add(authorIdParameter);
      cmd.ExecuteNonQuery();
      if (conn != null)
      {
        conn.Close();
      }
    }

    public void AddBook(Book newBook)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO copies (author_id, book_id, due_date, is_checkedOut) VALUES (@AuthorId, @bookId, @dueDate, @isCheckedOut);";
      MySqlParameter author_id = new MySqlParameter();
      author_id.ParameterName = "@AuthorId";
      author_id.Value = _id;
      cmd.Parameters.Add(author_id);
      MySqlParameter book_id = new MySqlParameter();
      book_id.ParameterName = "@BookId";
      book_id.Value = newBook.GetId();
      cmd.Parameters.Add(book_id);
      MySqlParameter due_date = new MySqlParameter();
      due_date.ParameterName = "@dueDate";
      due_date.Value = newBook.GetDueDate();
      cmd.Parameters.Add(due_date);
      MySqlParameter is_checkedOut = new MySqlParameter();
      is_checkedOut.ParameterName = "@isCheckedOut";
      is_checkedOut.Value = newBook.GetCheckedOut();
      cmd.Parameters.Add(is_checkedOut);
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
  }
}
