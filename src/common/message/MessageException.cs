namespace Messenger.Common
{
    public class MessageException : Exception
    {
        public string TheMessage { get; private set; }
        public MessageException(string message)
        {
            TheMessage = message;
        }
    }
}