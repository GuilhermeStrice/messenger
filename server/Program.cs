using Messenger.Common.Server;

namespace Server
{
    // test
    class Program
    {
        //static Messenger messenger = new Messenger();

        static void Main(string[] args)
        {
            //messenger.Init();

            /*var message = new Message();
            message.Command = "handshake";
            message.Content = "some_random_stuff";

            var serialized = message.Serialize();
            Console.WriteLine(serialized);

            var message_reversed = Message.Deserialize(serialized);
            Console.WriteLine(message_reversed.Command);
            Console.WriteLine(message_reversed.Content);*/

            var trusted_client = new TrustedClient("12.12.12.12", "id_potente", "Servidor 1");
            File.WriteAllText("test_id", trusted_client.Serialize());
            Console.WriteLine(trusted_client.Serialize());
        }
    }
}