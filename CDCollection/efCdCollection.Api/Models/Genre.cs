namespace efCdCollection.Api.Models;

public class Genre
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public List<CD>? CDs { get; set; }
}