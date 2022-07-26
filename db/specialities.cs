using System.ComponentModel.DataAnnotations;

namespace db_namespace;

public class Speciality
{
    [Key] public int Id { get; set; }

    [Required, MaxLength(4)] public string Code { get; set; }
    [Required] public string Name { get; set; }
    
    [Required] public int FacultyId { get; set; }
    public Faculty Faculty { get; set; } 
}