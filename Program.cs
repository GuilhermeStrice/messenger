using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Messenger
{
    class Program
    {
        static TcpListener listener = new TcpListener(IPAddress.Any, 2323);

        static List<Connection> connections = new List<Connection>();

        static List<TrustedClient> trusted_clients = new List<TrustedClient>();

        static bool test = true;

        static void Main(string[] args)
        {
            string trusted_clients_file = "trusted_clients";
            if (!File.Exists(trusted_clients_file))
            {
                Console.WriteLine("Trusted clients file does not exist. Creating.");
                File.Create(trusted_clients_file);

                Console.WriteLine("Please add at least one trusted client to the file to continue");
                Environment.Exit(0);
            }

            string[] trusted_clients_file_contents = File.ReadAllLines(trusted_clients_file);

            for (int i = 0; i < trusted_clients_file_contents.Length; i++)
            {
                try
                {
                    trusted_clients.Add(TrustedClient.Deserialize(trusted_clients_file_contents[i]));
                }
                catch
                {
                    Console.WriteLine("Trusted Client on line " + (i + 1) + " is not valid. Terminating");
                    Environment.Exit(0);
                }
            }

            if (!test)
            {
                listener.Start();
                Console.WriteLine("Listening to possible connections");

                while (true)
                {
                    if (listener.Pending())
                    {
                        Console.WriteLine("Client connection received");
                        var client = listener.AcceptTcpClient();

                        var connection = new Connection(client);
                        
                        try
                        {
                            Console.WriteLine("Handshaking: " + ((IPEndPoint)(client.Client.RemoteEndPoint)).Address.ToString());
                        
                            connection.Handshake();
                            connections.Add(connection);
                        }
                        catch
                        {
                            Console.WriteLine("Client connection dropped before handshake");
                        }
                    }

                    for (int i = 0; i < connections.Count(); i++)
                    {
                        Connection con = connections[i];

                        if (con.Terminated)
                        {
                            connections.RemoveAt(i); // just remove the connection
                            continue;
                        }

                        if (!string.IsNullOrEmpty(con.client_identifier) && !con.IsTrusted)
                        {
                            var received_trusted_client = TrustedClient.Deserialize(con.client_identifier);
                            
                            if (!received_trusted_client.IsValid())
                                con.Terminate();
                            
                            // check if is trusted
                            for (int j = 0; j < trusted_clients.Count(); j++)
                            {
                                if (trusted_clients[j] == received_trusted_client)
                                {
                                    con.IsTrusted = true;
                                    break; // exit j loop
                                }
                            }

                            // is not in trusted list
                            if (!con.IsTrusted)
                            {
                                // just terminate
                                con.Terminate();
                            }
                        }

                        con.Handle();

                        if (con.Messages.Count() > 0)
                        {
                            var message = con.Messages.Dequeue();
                        }
                    }
                }
            }
            else
            {
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
}