using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace efCdCollection.Api.Models;

[Index(nameof(Name), IsUnique = true)]
public class Genre
{   
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
}