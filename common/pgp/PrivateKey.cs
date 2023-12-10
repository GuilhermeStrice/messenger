using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Messenger.Common.Pgp
{
    public class PrivateKey
    {
        public PgpSecretKey? Key { get; set; }
        public string Passphrase { get; set; }

        public PrivateKey()
        {
            Key = null;
            Passphrase = "";
        }

        // we dont need PrivateKey string, but anyway
        public override string ToString()
        {
#pragma warning disable CS8604
            return PgpManager.Keys.Export(Key);
#pragma warning restore CS8604
        }

        public bool IsValidKey()
        {
            return Key != null && !string.IsNullOrEmpty(Passphrase);
        }

        public void Reset()
        {
            Key = null;
            Passphrase = "";
        }
    }
}