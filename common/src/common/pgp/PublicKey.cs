using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Messenger.Common.Pgp
{
    public class PublicKey
    {
        public PgpPublicKey? Key { get; set; }

        public override string ToString()
        {
#pragma warning disable CS8604
            return PgpManager.Keys.Export(Key);
#pragma warning restore CS8604
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