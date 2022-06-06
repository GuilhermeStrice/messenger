using System.Net.Sockets;
using System.Text;
using Pericia.OpenPgp;

namespace Messenger
{
    public class Connection
    {
        TcpClient client;
        Key serverKey;
        Key clientKey;

        string tempMessage = "";

        public bool Terminated = false;

        public Queue<Message> Messages = new Queue<Message>();

        int message_count = 0;

        public string client_identifier = "";

        public bool IsTrusted { get; set; }

        public Connection(TcpClient clientSocket)
        {
            serverKey = new Key();
            clientKey = new Key();
            client = clientSocket;
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

        public void Handshake()
        {
            serverKey = PgpManager.GenerateKey(Util.RandomString(20), Util.RandomString(20));
            clientKey.Reset();
            
            var stream = client.GetStream();

            var message = new Message();
            message.Command = "handshake";
            message.Content = PgpManager.ExportPublicKey(serverKey);
            
            var messageBytes = Encoding.UTF8.GetBytes(message.Serialize());
            
            stream.Write(messageBytes, 0, messageBytes.Length);

            // wait for client public key
        }

        public void Handle()
        {
            if (Terminated)
            {
                Messages.Clear();
                return;
            }

            try
            {
                var stream = client.GetStream();
                if (stream.DataAvailable)
                {
                    byte[] arr = new byte[512];
                    int messageSize = stream.Read(arr, 0, 512);

                    var messagePiece = Encoding.UTF8.GetString(arr, 0, messageSize);
                    tempMessage += messagePiece;
                }

                if (Message.StringContainsMessage(tempMessage))
                {
                    // we got a valid message
                    Message real_msg;
                    try
                    {
                        Message.FilterMessage(tempMessage, out real_msg, out tempMessage);
                    }
                    catch (MessageException ex)
                    {
                        Console.WriteLine("Invalid Message Received: " + ex.TheMessage);
                        return;
                    }

                    if (serverKey.IsValidServerKey() && clientKey.IsValidClientKey() && IsTrusted)
                    {
                        message_count++;
                        Messages.Enqueue(real_msg);
                    }
                    else
                    {
                        if (real_msg.Command == "handshake")
                        {
                            IOpenPgpEncryption pgp = new OpenPgpEncryption();

                            string clientPublicKey = pgp.Decrypt(real_msg.Content, serverKey.PrivateKey, serverKey.Passphrase);
                            clientKey.PublicKey = PgpManager.LoadPublicKey(clientPublicKey);

                            // after receiving the client public key
                            // we send the server public key
                            // encrypted with the client public key

                            Message handshake = new Message();
                            handshake.Command = "tr4";
                            handshake.Content = "";

                            byte[] handshakeMessageBytes = Encoding.UTF8.GetBytes(handshake.Serialize());
                            stream.Write(handshakeMessageBytes, 0, handshakeMessageBytes.Length);
                        }
                        else if (real_msg.Command == "tr4")
                        {
                            IOpenPgpEncryption pgp = new OpenPgpEncryption();

                            client_identifier = pgp.Decrypt(real_msg.Content, serverKey.PrivateKey, serverKey.Passphrase);
                        }
                        else if (real_msg.Command == "terminate")
                        {
                            // also probably do other stuff

                            Terminated = true;
                        }
                        else
                        {
                            // illegal
                            Terminated = true;
                        }
                    }
                }

                // else wait for next round of data

                if (message_count > 5) // after 5 messages we re-handshake
                    Handshake();
            }
            catch
            {
                // we probably lost connection
                Terminate();
            }
        }
    }
}