using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryDB.Models;

[Table("library_books")]
public class LibraryBookDb(int libraryId, int bookId, int availableCount)
{
    [Column("library_id")]
    public int LibraryId { get; init; } = libraryId;
    
    [Column("book_id")]
    public int BookId { get; init; } =  bookId;
    
    [Column("available_count")]
    public int AvailableCount { get; set; } = availableCount;
}