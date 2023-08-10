namespace PITB.CMS.Models.View
{
    public class VmMessage
    {
        public VmMessage() { }

        public VmMessage(string message,Config.MessageType type)
        {
            MessageText = message;
            Type = type;
        }

        public string  MessageText { get; set; }
        public Config.MessageType Type { get; set; }
    }
}