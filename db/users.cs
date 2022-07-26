using System.ComponentModel.DataAnnotations;

namespace db_namespace;

public enum StatusEnum
{
    Normal=0,
    Banned=1
}

public enum RoleEnum
{
    User=0,
    Moderator=1,
    Admin=2,
    SuperAdmin=3
}

public class User
{
    [Key, MaxLength(12)]
    public int Id { get; set; }
    public string? Name { get; set; }
    public StatusEnum Status { get; set; }
    public RoleEnum Role { get; set; }
    [Range(1, 6)] public int Course { get; set; }

    public int FacultyId { get; set; }
    public int SpecialityId { get; set; }
    
    public Speciality? Speciality { get; set; }
    public Faculty? Faculty { get; set; }
}