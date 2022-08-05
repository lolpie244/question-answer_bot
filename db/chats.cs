using System.ComponentModel.DataAnnotations;

namespace db_namespace;

public enum ChatEnum
{
    Answer = 0,
    Archive = 1
}

public class Chat
{
    [Key, MaxLength(12)] 
    public long Id { get; set; }
    public ChatEnum Type { get; set; }
}