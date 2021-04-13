using Certes.Acme.Resource;
using CommandLine;

namespace CinderBlockGames.GitHub.Actions.LERevoke
{
    public class Options
    {

        [Option("acmeAccountKeyPath",
            Required = false,
            HelpText = "The file holding the key associated with the account to use when communicating with Let's Encrypt.  REQUIRED if certKeyPath is not provided.")]
        public string AcmeAccountKeyPath { get; set; }

        [Option("certChainPath",
            Required = true,
            HelpText = "The file holding the certificate to be revoked (and its chain).")]
        public string CertificateChainPath { get; set; }

        [Option("certKeyPath",
            Required = false,
            HelpText = "The file holding the plaintext private key for the certificate being revoked.  REQUIRED if acmeAccountKeyPath is not provided.")]
        public string PrivateKeyPath { get; set; }

        [Option("reason",
            Required = false, Default = RevocationReason.Unspecified,
            HelpText = "The reason the certificate is being revoked.  See options at https://github.com/fszlin/certes/blob/master/src/Certes/Acme/Resource/RevocationReason.cs.")]
        public RevocationReason RevocationReason { get; set; }

    }
}