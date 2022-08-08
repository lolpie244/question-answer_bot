using System.ComponentModel.DataAnnotations;
using Telegram.Bot.Types;

namespace db_namespace;

public class Archive
{
    [Key]
    public int Id { get; set; }
    
    public long AskerId { get; set; }
    
    public int MessageId { get; set; }
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public bool IsQuestion { get; set; } = false;

}