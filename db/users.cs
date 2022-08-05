using System.ComponentModel.DataAnnotations;

namespace db_namespace;

public enum RoleEnum
{
    Banned=-1,
    User=0,
    Moderator=1,
    Admin=2,
    SuperAdmin=3
}

public class User
{
    [Key, MaxLength(12)]
    public long Id { get; set; }
    public string? Name { get; set; }
    public RoleEnum Role { get; set; }

    public UserData? UserSettings { get; set; }
}