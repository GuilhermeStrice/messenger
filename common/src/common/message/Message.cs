using Messenger.Common.Pgp;

namespace Messenger.Common.Messaging
{
    public class Message
    {
        public string Command { get; set; }

        public InnerMessage? InnerMessage { get; set; }

        public Message()
        {
            Command = "";
        }

        public string Serialize(PublicKey key)
        {
            if (string.IsNullOrEmpty(Command) && InnerMessage != null)
                throw new MessageException("Command or content empty");
            string msg = "€";
            msg += "command:";
            msg += Command;
            msg += ";";
            msg += "content:";
#pragma warning disable CS8602
#pragma warning disable CS8604
            msg += PgpManager.Pgp.Encrypt(InnerMessage.Serialize(), key.Key);
#pragma warning restore CS8602
#pragma warning restore CS8602
            msg += ";$";

            return msg;
        }

        public static Message Deserialize(string message, PrivateKey key)
        {
            var real_message = message.Substring(1, message.Length - 3); // remove first and last two characters

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
            msg.InnerMessage = InnerMessage.Deserialize(PgpManager.Pgp.Decrypt(content_parts[1],
#pragma warning disable CS8604
                                                        key.Key, 
#pragma warning restore CS8604
                                                        key.Passphrase));

            return msg;
        }

        public static bool StringContainsMessage(string msg)
        {
            return msg.StartsWith('€') && msg.Contains('$');
        }
    }
}