using System.Collections.Concurrent;

namespace Messenger.Common
{
    // we only want one message queue per application
    public static class MessageQueue
    {
        private static ConcurrentQueue<Message> Messages = new ConcurrentQueue<Message>();

        public static bool TryGetNextMessage(out Message msg)
        {
            var can_i = Messages.TryDequeue(out msg);
            return can_i;
        }

        public static void AddMessage(Message msg)
        {
            Messages.Enqueue(msg);
        }
    }
}