using Infinity.Core.Udp;
using Messenger.Common;
using System.Net;

namespace Server
{
    class Program
    {
        static UdpConnectionListener listener;

        static void Main(string[] args)
        {
            listener = new UdpConnectionListener(new IPEndPoint(IPAddress.Any, 25515));

            listener.HandshakeConnection += (sender, e) =>
            {

            };

            listener.NewConnection += Listener_NewConnection;

            listener.Start();

            /*var message = new Message();
            message.Command = "handshake";
            message.Content = "some_random_stuff";

            var serialized = message.Serialize();
            Console.WriteLine(serialized);

            var message_reversed = Message.Deserialize(serialized);
            Console.WriteLine(message_reversed.Command);
            Console.WriteLine(message_reversed.Content);*/

            var trusted_client = new TrustedClient("12.12.12.12", Util.RandomString(30), "Servidor 1");
            File.WriteAllText("test_id", trusted_client.Serialize());
            Console.WriteLine(trusted_client.Serialize());
        }

        private static void Listener_NewConnection(Infinity.Core.NewConnectionEventArgs obj)
        {
            throw new NotImplementedException();
        }
    }
}