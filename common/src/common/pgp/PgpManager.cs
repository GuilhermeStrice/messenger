using Org.BouncyCastle.Bcpg.OpenPgp;
using Pericia.OpenPgp;

namespace Messenger.Common.Pgp
{
    public static class PgpManager
    {
        public static IOpenPgpEncryption Pgp = new OpenPgpEncryption();
        public static IOpenPgpKeyManagement Keys = new OpenPgpKeyManagement();

        

        public static PgpSecretKey GenerateKey(string identity, string passphrase)
        {
            return Keys.GenerateKeyPair(identity, passphrase);
        }

        public static PgpSecretKey LoadPrivateKey(string privateKey)
        {
            return Keys.LoadSecretKey(privateKey);
        }

        public static PgpPublicKey LoadPublicKey(string publicKey)
        {
            return Keys.LoadPublicKey(publicKey);
        }

        public static string ExportPrivateKey(PgpSecretKey key)
        {
            return Keys.Export(key);
        }

        public static string ExportPublicKey(PgpPublicKey key)
        {
            return Keys.Export(key);
        }
    }
}