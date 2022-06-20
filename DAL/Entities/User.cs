namespace DAL.Entities;
public enum RoleNames
{
    Admin,
    Manager,
    User,
}
public class User 
{
    public Guid Id { get; set; }
    public string EMail { get; set; }
    public string Name { get; set; }
    public RoleNames Role { get; set; }
    public string Password { get; set; }
}

