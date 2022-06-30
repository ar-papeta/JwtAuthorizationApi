namespace DAL.Entities;

public class User : IDocument
{
    public string Id { get; set; } = null!;
    public string EMail { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Password { get; set; } = null!;
}

