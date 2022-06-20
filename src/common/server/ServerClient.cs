using System.Net.Sockets;
using System.Text;

using Messenger.Common.Messaging;
using Messenger.Common.Pgp;

namespace Messenger.Common.Server
{
    public class ServerClient
    {
        TcpClient client;
        NetworkStream client_stream;

        PrivateKey ServerPrivateKey = new PrivateKey();
        PublicKey ServerPublicKey = new PublicKey();

        PublicKey ClientPublicKey = new PublicKey();

        public bool HasHandshaked = false;

        string tempMessage = "";

        public bool Terminated = false;

        public TrustedClient? ClassIdentifier;

        public Trustworthiness Trustworthiness = Trustworthiness.NotChecked;

        public ServerClient(TcpClient clientSocket)
        {
            GenerateServerKey();

            client = clientSocket;
            client_stream = client.GetStream();
        }

        private void GenerateServerKey()
        {
            var passphrase = Util.RandomString(20);
            var key = PgpManager.GenerateKey(Util.RandomString(20), passphrase);

            ServerPrivateKey.Reset();
            ServerPrivateKey.Key = key;
            ServerPrivateKey.Passphrase = passphrase;

            ServerPublicKey.Reset();
            ServerPublicKey.Key = ServerPrivateKey.Key.PublicKey;

            ClientPublicKey.Reset();
        }

        public void Terminate()
        {
            try
            {
                client.Close();
            }
            catch {} // i dont care what happens here

            Terminated = true;
        }

        private async Task Handshake()
        {
            GenerateServerKey();
            
            client_stream = client.GetStream();

            var message = new InnerMessage(); // no need for full message
            message.Command = "handshake";
            message.Content = ServerPublicKey.ToString();

            await PassMessage(message);

            // wait for client public key
        }

        private async Task ReadData()
        {
            if (client_stream.DataAvailable)
            {
                byte[] arr = new byte[64];
                int messageSize = await client_stream.ReadAsync(arr, 0, 64);

                var messagePiece = Encoding.UTF8.GetString(arr, 0, messageSize);
                tempMessage += messagePiece;
            }
        }

        public async Task PassMessage(Message msg)
        {
            var serialized_msg = msg.Serialize(ClientPublicKey);
            var bytes = Encoding.UTF8.GetBytes(serialized_msg);

            await client_stream.WriteAsync(bytes, 0, bytes.Count());
        }

        private async Task PassMessage(InnerMessage msg)
        {
            var serialized_msg = msg.Serialize(ClientPublicKey);
            var bytes = Encoding.UTF8.GetBytes(serialized_msg);

            await client_stream.WriteAsync(bytes, 0, bytes.Count());
        }

        public async Task Handle()
        {
            if (Terminated)
            {
                return;
            }

            try
            {
                await ReadData();

                // first handshake
                if (!HasHandshaked)
                    await Handshake();

                if (Message.StringContainsMessage(tempMessage))
                {
                    // we got a valid message
                    string real_msg;
                    try
                    {
                        MessageUtil.FilterMessage(tempMessage, out real_msg, out tempMessage);
                    }
                    catch (MessageException ex)
                    {
                        Console.WriteLine("Invalid Message Received: " + ex.TheMessage);
                        return;
                    }

                    if (ServerPrivateKey.IsValidKey() && 
                        ServerPublicKey.IsValidKey() && 
                        ClientPublicKey.IsValidKey() && 
                        Trustworthiness == Trustworthiness.Trusted)
                    {
                        var deserialized_msg = Message.Deserialize(real_msg, ServerPrivateKey);
                        MessageQueue.AddMessage(deserialized_msg);
                    }
                    else
                    {
                        var deserialized_msg = InnerMessage.Deserialize(real_msg);
                        if (deserialized_msg.Command == "handshake")
                        {
                            if (HasHandshaked)
                            {
                                // bad behaviour - what should i do?
                                return;
                            }

                            HasHandshaked = true;
#pragma warning disable CS8604
                            string clientPublicKey = PgpManager.Pgp.Decrypt(deserialized_msg.Content, ServerPrivateKey.Key, ServerPrivateKey.Passphrase);
#pragma warning restore CS8604
                            ClientPublicKey.Key = PgpManager.LoadPublicKey(clientPublicKey);

                            if (Trustworthiness == Trustworthiness.NotChecked)
                            {
                                // get identifier
                                var handshake = new InnerMessage();
                                handshake.Command = "tr4";
                                handshake.Content = "";

                                await PassMessage(handshake);
                            }
                        }
                        else if (deserialized_msg.Command == "tr4")
                        {
                            if (!HasHandshaked && (object?)ClassIdentifier != null)
                            {
                                // bad behaviour - what should i do?
                                return;
                            }
#pragma warning disable CS8604
                            var client_identifier = PgpManager.Pgp.Decrypt(deserialized_msg.Content, ServerPrivateKey.Key, ServerPrivateKey.Passphrase);
#pragma warning restore CS8604
                            ClassIdentifier = TrustedClient.Deserialize(client_identifier);
                        }
                        else if (deserialized_msg.Command == "terminate")
                        {
                            // also probably do other stuff

                            Terminate();
                        }
                        else
                        {
                            // illegal
                            Terminate();
                        }
                    }
                    
                    await Handshake(); // every message re-handshake
                }

                // else wait for next round of data
            }
            catch
            {
                // we probably lost connection
                Terminate();
            }
        }
    }
}