using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Messenger.Common
{
    public class PrivateKey
    {
        public PgpSecretKey Key { get; set; }
        public string Passphrase { get; set; }

        // we dont need PrivateKey string, but anyway
        public override string ToString()
        {
            return PgpManager.Keys.Export(Key);
        }

        public bool IsValidKey()
        {
            return Key != null && !string.IsNullOrEmpty(Passphrase);
        }

        public void Reset()
        {
            Key = null;
            Passphrase = null;
        }
    }
}