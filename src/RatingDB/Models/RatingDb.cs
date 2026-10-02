using System.ComponentModel.DataAnnotations.Schema;

namespace RatingDB.Models;

[Table("rating")]
public class RatingDb(int id, string username, int stars)
{
    [Column("id")]
    public int Id { get; init; } = id;
    
    [Column("username")]
    public string Username { get; init; } = username;

    [Column("stars")]
    public int Stars { get; set; } = stars;
}