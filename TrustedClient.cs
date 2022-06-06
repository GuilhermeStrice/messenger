using System.Text;

namespace Messenger
{
    public class TrustedClient
    {
        public static string SeparatorBase64 = "4oKs"; // €

        public string IP { get; private set; }
        public string ServerIdentifier { get; private set; }
        public string PublicName { get; private set; }

        public TrustedClient(string ip, string server_id, string public_name)
        {
            IP = ip;
            ServerIdentifier = server_id;
            PublicName = public_name;
        }

        public string Serialize()
        {
            if (!IsValid())
                return "";
            var ip_bytes = Encoding.UTF8.GetBytes(IP);
            var server_id_bytes = Encoding.UTF8.GetBytes(ServerIdentifier);
            var name_bytes = Encoding.UTF8.GetBytes(PublicName);

            var ip_base64 = Convert.ToBase64String(ip_bytes);
            var server_id_base64 = Convert.ToBase64String(server_id_bytes);
            var name_base64 = Convert.ToBase64String(name_bytes);

            return ip_base64 + SeparatorBase64 + server_id_base64 + SeparatorBase64 + name_base64;
        }

        public static TrustedClient Deserialize(string base64_str)
        {
            try
            {
                var parts = base64_str.Split(SeparatorBase64);
                
                var ip_bytes = Convert.FromBase64String(parts[0]);
                var server_id_bytes = Convert.FromBase64String(parts[1]);
                var name_bytes = Convert.FromBase64String(parts[2]);

                var ip = Encoding.UTF8.GetString(ip_bytes);
                var server_id = Encoding.UTF8.GetString(server_id_bytes);
                var name = Encoding.UTF8.GetString(name_bytes);
                
                var trusted_client = new TrustedClient(ip, server_id, name);
                return trusted_client;
            }
            catch
            {
                return new TrustedClient("", "", "");
            }
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(IP) &&
                !string.IsNullOrEmpty(ServerIdentifier) &&
                !string.IsNullOrEmpty(PublicName);
        }

        public static bool operator==(TrustedClient right, TrustedClient left)
        {
            return right.IP == left.IP &&
                right.ServerIdentifier == left.ServerIdentifier &&
                right.PublicName == left.PublicName;
        }

        public static bool operator!=(TrustedClient right, TrustedClient left)
        {
            return right.IP != left.IP ||
                right.ServerIdentifier != left.ServerIdentifier ||
                right.PublicName != left.PublicName;
        }
    }
}