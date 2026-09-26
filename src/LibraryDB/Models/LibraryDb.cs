using System.ComponentModel.DataAnnotations.Schema;
using LibraryBL.Models;

namespace LibraryDB.Models;

[Table("library")]
public class LibraryDb(int id, Guid libraryUid, string name, string city, string address)
{
    public LibraryDb(Library library) : this(library.Id, library.LibraryUid, library.Name, library.City, library.Address) {}

    [Column("id")]
    public int Id { get; init; } = id;
    
    [Column("library_uid")]
    public Guid LibraryUid { get; init; } = libraryUid;
    
    [Column("name")]
    public string Name { get; init; } = name;
    
    [Column("city")]
    public string City { get; init; } = city;
    
    [Column("address")]
    public string Address { get; init; } = address;
    
    public Library ToBlModel() => new Library(Id, LibraryUid, Name, City, Address);
}