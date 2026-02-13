namespace Controller.Interfaces;

public abstract class RoleAware
{
    protected string Role { get; set; }

    protected RoleAware(string role)
    {
        Role = role;
    }

    public void SetRole(string role)
    {
        Role = role;
    }

    protected bool IsAdmin =>
        Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
}