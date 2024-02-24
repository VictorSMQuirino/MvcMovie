namespace MvcMovie.Models;

public class Artist
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Bio { get; set; }
    public required string Site { get; set; }
    public ICollection<Movie>? MovieArtist { get; set; }
}
