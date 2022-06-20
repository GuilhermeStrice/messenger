namespace Messenger.Common.Messaging
{
    public static class MessageUtil
    {
        public static void FilterMessage(string text, out string msg, out string the_rest)
        {
            int endIndex = text.IndexOf('$');
            msg = text.Substring(0, endIndex + 1);
            the_rest = text.Substring(endIndex, text.Length - endIndex + 1);
        }
    }
}