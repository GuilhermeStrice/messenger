using Org.BouncyCastle.Bcpg.OpenPgp;
using Pericia.OpenPgp;

namespace Messenger
{
    public static class PgpManager
    {
        private static IOpenPgpEncryption pgp = new OpenPgpEncryption();
        private static IOpenPgpKeyManagement keys = new OpenPgpKeyManagement();

        public static Key GenerateKey(string identity, string passphrase)
        {
            var keyPair = keys.GenerateKeyPair(identity, passphrase);

            var key = new Key();
            key.PrivateKey = keyPair;
            key.PublicKey = keyPair.PublicKey;
            key.Passphrase = passphrase;

            return key;
        }

        public static PgpSecretKey LoadPrivateKey(string privateKey)
        {
            return keys.LoadSecretKey(privateKey);
        }

        public static PgpPublicKey LoadPublicKey(string publicKey)
        {
            return keys.LoadPublicKey(publicKey);
        }

        public static string ExportPrivateKey(Key key)
        {
            return keys.Export(key.PrivateKey);
        }

        public static string ExportPublicKey(Key key)
        {
            return keys.Export(key.PublicKey);
        }
    }
}