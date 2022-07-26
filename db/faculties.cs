using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic.FileIO;

namespace db_namespace;

public class Faculty
{
    [Key] public int Id { get; set; }
    public string Name { get; set; }
    public List<Speciality> Specialities { get; set; }
}