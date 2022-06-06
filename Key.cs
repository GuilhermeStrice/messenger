using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Messenger
{
    public class Key
    {
        public PgpSecretKey? PrivateKey { get; set; } = null;
        public PgpPublicKey? PublicKey { get; set; } = null;

        public string? Passphrase { get; set; } = null;

        public Key()
        {
        }

        public Key(string key, bool isPublic)
        {
            if (string.IsNullOrEmpty(key))
                return;
            if (!isPublic)
            {
                PrivateKey = PgpManager.LoadPrivateKey(key);
                PublicKey = PrivateKey.PublicKey;
            }
            else
                PublicKey = PgpManager.LoadPublicKey(key);
        }

        public bool IsValidServerKey()
        {
            return PrivateKey != null && !string.IsNullOrEmpty(Passphrase) && PublicKey != null;
        }

        public bool IsValidClientKey()
        {
            return PrivateKey == null && PublicKey != null;
        }

        public void Reset()
        {
            PrivateKey = null;
            PublicKey = null;
        }
    }
}