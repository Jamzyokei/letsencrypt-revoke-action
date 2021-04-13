using System;
using System.IO;
using System.Threading.Tasks;
using Certes;
using Certes.Acme;
using Certes.Acme.Resource;
using CommandLine;

namespace CinderBlockGames.GitHub.Actions.LERevoke
{
    class Program
    {

        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(Run);
#if DEBUG
            Console.ReadLine(); // Pause for review.
#endif
        }

        private static async Task Run(Options options)
        {
            // Establish context.
            var key = (!string.IsNullOrWhiteSpace(options.AcmeAccountKeyPath) && File.Exists(options.AcmeAccountKeyPath))
                ? File.ReadAllText(options.AcmeAccountKeyPath)
                : null;
            var context = await GetContext(key);

            try
            {
                // Revoke certificate.
                await RevokeCertificate(
                    context,
                    options.CertificateChainPath,
                    options.PrivateKeyPath,
                    options.RevocationReason);
            }
            finally
            {
                // Try not to strand too many temporary accounts out there.
                if (context.Temporary)
                {
                    Console.WriteLine("Deactivating temporary ACME account...");
                    await DeactivateAccount(context.Value);
                }
            }

            Console.WriteLine("Complete!");
        }

        private static async Task RevokeCertificate(Context context, string chainPath, string keyPath, RevocationReason reason)
        {
            if (context.Temporary) {
                Console.WriteLine("Revoking certificate using private key...");
                var chain = new CertificateChain(File.ReadAllText(chainPath));
                var keyText = File.ReadAllText(keyPath);
                keyText = keyText.Substring(keyText.IndexOf("-----")); // The line below breaks if the attributes lines are present before the key.
                var key = KeyFactory.FromPem(keyText);
                await context.Value.RevokeCertificate(chain.Certificate.ToDer(), reason, key);
            }
            else
            {
                Console.WriteLine("Revoking certificate using account key...");
                var chain = new CertificateChain(File.ReadAllText(chainPath));
                await context.Value.RevokeCertificate(chain.Certificate.ToDer(), reason);
            }
        }

        private static async Task DeactivateAccount(IAcmeContext context)
        {
            var account = await context.Account();
            await account.Deactivate();
        }

        private static async Task<Context> GetContext(string key)
        {
            var server = WellKnownServers.LetsEncryptV2;
#if DEBUG
            server = WellKnownServers.LetsEncryptStagingV2;
#endif

            if (string.IsNullOrWhiteSpace(key))
            {
                Console.WriteLine("Creating temporary ACME account...");
                var acme = new AcmeContext(server);
                var account = await acme.NewAccount("temporary@cinderblockgames.com", true);
                return new Context { Value = acme, Temporary = true };
            }
            else
            {
                // Load the provided account key.
                Console.WriteLine("Loading ACME account key...");
                var pem = KeyFactory.FromPem(key);
                var acme = new AcmeContext(server, pem);
                var account = await acme.Account();
                return new Context { Value = acme, Temporary = false };
            }
        }

        private class Context
        {
            public IAcmeContext Value { get; set; }
            public bool Temporary { get; set; }
        }

    }
}