using System.ComponentModel.DataAnnotations.Schema;
using LibraryBL.Enums;
using LibraryBL.Models;

namespace LibraryDB.Models;

[Table("books")]
public class BookDb(int id, Guid bookUid, string name, string? author, string? genre, Condition condition)
{
    public BookDb(Book book) : this(book.Id, book.BookUid, book.Name, book.Author, book.Genre, book.Condition) {}

    [Column("id")]
    public int Id { get; init; } = id;
    
    [Column("book_uid")]
    public Guid BookUid { get; init; } = bookUid;
    
    [Column("name")]
    public string Name { get; init; } = name;
    
    [Column("author")]
    public string? Author { get; init; } = author;
    
    [Column("genre")]
    public string? Genre { get; init; } = genre;
    
    [Column("condition")]
    public Condition Condition { get; set; } = condition;
    
    public Book ToBlModel(int availableCount) => new Book(Id, BookUid, Name, Author, Genre, Condition, availableCount);
}