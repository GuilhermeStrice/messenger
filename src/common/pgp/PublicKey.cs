using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Messenger.Common
{
    public class PublicKey
    {
        public PgpPublicKey Key { get; set; }

        public override string ToString()
        {
            return PgpManager.Keys.Export(Key);
        }

        public bool IsValidKey()
        {
            return Key != null;
        }

        public void Reset()
        {
            Key = null;
        }
    }
}