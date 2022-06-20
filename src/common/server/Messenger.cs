using System.Net;
using System.Net.Sockets;

using Messenger.Helpers;
using Messenger.Common.Messaging;

namespace Messenger.Common.Server
{
    public class Messenger
    {
		TcpListener listener = new TcpListener(IPAddress.Any, 2323);

        ConcurrentList<ServerClient> connections = new ConcurrentList<ServerClient>();
        ConcurrentList<TrustedClient> trusted_clients = new ConcurrentList<TrustedClient>();

        public void Init()
        {
            trusted_clients = TrustedClient.ReadFileList("trusted_clients");

            listener.Start();
            Console.WriteLine("Listening to possible connections");
        }

        public async Task AcceptClients()
        {
            if (listener.Pending())
            {
                Console.WriteLine("Client connection received");
                var client = await listener.AcceptTcpClientAsync();

                var connection = new ServerClient(client);
                
                try
                {
#pragma warning disable CS8602
#pragma warning disable CS8600
                    Console.WriteLine("Will begin handshaking: " + ((IPEndPoint)(client.Client.RemoteEndPoint)).Address.ToString());
#pragma warning restore CS8602
#pragma warning restore CS8600
                    connections.Add(connection);
                }
                catch
                {
                    Console.WriteLine("Client connection dropped before handshake");
                }
            }
        }

        public async Task HandMessagePassing(ServerClient con)
        {
            Message msg;
            if (MessageQueue.TryGetNextMessage(out msg))
            {
                // this is to pass to another client

                bool passed = false;

                for (int j = 0; j < connections.Length; j++)
                {
                    // should be 
                    // €command=server_identifier;content:message;$
                    // TrustedClient.ServerIdentifier
#pragma warning disable CS8602
                    if (connections[j].ClassIdentifier.ServerIdentifier == msg.InnerMessage.Command)
#pragma warning restore CS8602                    
                    {
                        passed = true;
                    
                        await connections[j].PassMessage(msg);
                    }
                }

                if (!passed)
                {
                    // server_identifier wrong
#pragma warning disable CS8602
                    Console.WriteLine(msg.InnerMessage.Command + " - identifier doesnt exist");
#pragma warning restore CS8602
                }
            }
        }

        public void HandleTerminations(ServerClient con)
        {
            if (con.Terminated)
            {
                connections.Remove(con); // just remove the connection
                return;
            }
        }

        public async Task HandleData(ServerClient con)
        {
            if (con.ClassIdentifier != null && con.Trustworthiness == Trustworthiness.NotChecked)
            {
                // check if is trusted
                for (var tcI = 0; tcI < trusted_clients.Length; tcI++)
                {
                    var t = trusted_clients[tcI];
                    if (t == con.ClassIdentifier)
                    {
                        con.Trustworthiness = Trustworthiness.Trusted;
                        break;
                    }
                    else
                        con.Trustworthiness = Trustworthiness.NotTrusted;
                }

                // is not in trusted list
                if (con.Trustworthiness == Trustworthiness.NotTrusted)
                {
                    // just terminate and process next client
                    con.Terminate();
                    return;
                }
            }

            await con.Handle();

            /*var message = new Message();
            message.Command = "handshake";
            message.Content = "some_random_stuff";

            var serialized = message.Serialize();
            Console.WriteLine(serialized);

            var message_reversed = Message.Deserialize(serialized);
            Console.WriteLine(message_reversed.Command);
            Console.WriteLine(message_reversed.Content);*/

            //var trusted_client = new TrustedClient("12.12.12.12", "id_potente", "Servidor 1");
            //File.WriteAllText("test_id", trusted_client.Serialize());
            //Console.WriteLine(trusted_client.Serialize());
        }
    }
}