using System.ComponentModel.DataAnnotations;
namespace efCdCollection.Api.Models;

public class CD
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ArtistName { get; set; }
    public string? Description { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public virtual Genre? Genre { get; set;}
}