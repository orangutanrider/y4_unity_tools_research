using UnityEditor;

public class HelpMessageData 
{
    public HelpMessageData(string _message, MessageType _messageType, bool _active = true)
    {
        message = _message;
        messageType = _messageType;
        active = _active;
    }


    public bool active = false; 
    // it is probably best to not use this active thing in the refactor as someone implementing a message returner, might just make it not return messages, instead of having it return inactive ones
    public string message = "NULL"; // In refactor change to use GUI Content
    public MessageType messageType = MessageType.None;
}
