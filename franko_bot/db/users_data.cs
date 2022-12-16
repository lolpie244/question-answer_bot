using System.ComponentModel.DataAnnotations;

namespace db_namespace;

public class UserData
{
    [Key, MaxLength(7)] public string Code { get; set; }
    public string Faculty { get; set; }
    public string Speciality { get; set; }
    [Range(1, 6)] public int Course { get; set; }
    public int Group { get; set; }
}