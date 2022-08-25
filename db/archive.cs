using System.ComponentModel.DataAnnotations;
using Telegram.Bot.Types;

namespace db_namespace;

public enum MessageType
{
    QA = 0,
    Report = 1
}
public class Archive
{
    public Archive(int messageId, long chatId, long userId) {}
    public Archive(Message message)
    {
        MessageId = message.MessageId;
        ChatId = message.Chat.Id;
        UserId = message.From!.Id;
    }
    [Key]
    public int Id { get; set; }
    public int MessageId { get; set; }
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public MessageType Type { get; set; }
    
    public long? RelatedUserId { get; set; }

    public bool IsQuestion
    {
        get => Type == MessageType.QA && RelatedUserId == UserId;
    }
}