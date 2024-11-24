namespace NSE.Core.Messages;

public class Message
{
    public string MessageType { get; protected set; }
    public int AggregateId { get; protected set; }

    public Message()
    {
        MessageType = GetType().Name;
    }
}