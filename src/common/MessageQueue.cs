using System.Collections.Concurrent;
using Messenger.Common.Messaging;

namespace Messenger.Common
{
    // we only want one message queue per application
    public static class MessageQueue
    {
        private static ConcurrentQueue<Message> Messages = new ConcurrentQueue<Message>();

        public static bool TryGetNextMessage(out Message msg)
        {
#pragma warning disable CS8601
            var can_i = Messages.TryDequeue(out msg);
#pragma warning disable CS8601
            return can_i;
        }

        public static void AddMessage(Message msg)
        {
            Messages.Enqueue(msg);
        }
    }
}