namespace Messenger
{
    public class MessageException : Exception
    {
        public string TheMessage { get; private set; }
        public MessageException(string message)
        {
            TheMessage = message;
        }
    }

    public class Message
    {
        public string Command { get; set; }
        public string Content { get; set; }

        public Message()
        {
            Command = "";
            Content = "";
        }

        public string Serialize()
        {
            return "€command:" + Command + ";content:" + Content + ";$";
        }

        public static Message Deserialize(string message)
        {
            var real_message = message.Substring(1, message.Count() - 3); // remove first and last two characters

            var real_message_parts = real_message.Split(';');

            if (real_message_parts.Length == 0)
                throw new MessageException(message);

            var command_parts = real_message_parts[0].Split(':');
            var content_parts = real_message_parts[1].Split(':');

            if (command_parts.Length == 0 || content_parts.Length == 0)
                throw new MessageException(message);

            if (command_parts[0] != "command" && content_parts[0] != "content")
                throw new MessageException(message);

            var msg = new Message();
            msg.Command = command_parts[1];
            msg.Content = content_parts[1];

            return msg;
        }

        public static bool StringContainsMessage(string msg)
        {
            return msg.StartsWith('€') && msg.Contains('$');
        }

        public static void FilterMessage(string text, out Message msg, out string the_rest)
        {
            int endIndex = text.IndexOf('$');
            var message = text.Substring(0, endIndex + 1);
            the_rest = text.Substring(endIndex, text.Length - endIndex + 1);
            msg = Message.Deserialize(message);
        }
    }
}