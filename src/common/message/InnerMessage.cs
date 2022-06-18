namespace Messenger.Common
{
    public class InnerMessage
    {
        public string Command { get; set; }
        public string Content { get; set; }

        public string Serialize()
        {
            if (string.IsNullOrEmpty(Command) || string.IsNullOrEmpty(Content))
                throw new MessageException("Command or content empty");
            return "€command:" + Command + ";content:" + Content + ";$";
        }

        public string Serialize(PublicKey key)
        {
            if (string.IsNullOrEmpty(Command) || string.IsNullOrEmpty(Content))
                throw new MessageException("Command or content empty");
            return "€command:" + Command + ";content:" + PgpManager.Pgp.Encrypt(Content, key.Key) + ";$";
        }

        public static InnerMessage Deserialize(string message)
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

            var msg = new InnerMessage();
            msg.Command = command_parts[1];
            msg.Content = content_parts[1];

            return msg;
        }
    }
}