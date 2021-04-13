# Revoke Certificate Issued by Let''s Encrypt
This action uses either an account key or a private key to revoke a certificate issued by Let's Encrypt.

## Inputs
| Parameter          | Required      | Default         | Description                                                                                                                                             |
| :---               | :---          | :---            | :---                                                                                                                                                    |
| acmeAccountKeyPath | **Sometimes** |                 | The file holding the key associated with the account to use when communicating with Let's Encrypt.  REQUIRED if certKeyPath is not provided.            |
| **certChainPath**  | **Yes**       |                 | The file holding the certificate to be revoked (and its chain).                                                                                         |
| certKeyPath        | **Sometimes** |                 | The file holding the plaintext private key for the certificate being revoked.  REQUIRED if acmeAccountKeyPath is not provided.                          |
| reason             | No            | **Unspecified** | The reason the certificate is being revoked.  See options at https://github.com/fszlin/certes/blob/master/src/Certes/Acme/Resource/RevocationReason.cs. |

## Example Workflow
```
# Workflow name
name: Revoke Certificate

# Controls when the action will run.
on:
  # Allows you to run this workflow manually from the Actions tab
  workflow_dispatch:

jobs:
  revoke-cert:
    runs-on: ubuntu-latest
    steps:
      - name: Copy ACME account key
        uses: DamianReeves/write-file-action@v1.0
        with:
          path: 'path/to/acme.key'
          contents: ${{ secrets.ACME_ACCOUNT_KEY }}
          write-mode: overwrite
      - name: Copy certificate chain
        uses: DamianReeves/write-file-action@v1.0
        with:
          path: 'path/to/cert.cer'
          contents: ${{ secrets.CERT_PUBLIC_CHAIN }}
          write-mode: overwrite
      - name: Copy certificate private key
        uses: kitek/decode-base64-into-file-action@1.0
        with:
          encoded-value: ${{ secrets.CERT_PRIVATE_KEY }}
          destination-file: 'path/to/cert.pfx'
      - name: Remove private key protection
        run: openssl pkcs12 -in ${{ github.workspace }}/path/to/cert.pfx' -nocerts -out ${{ github.workspace }}/path/to/cert.key -nodes -password pass:'${{ secrets.CERT_PASSWORD }}'
      - name: Revoke certificate
        uses: cinderblockgames/letsencrypt-revoke-action@v1.0.0
        with:
          # REQUIRED
          certChainPath: 'path/to/cert.cer'

          # ONE REQUIRED
          acmeAccountKeyPath: 'path/to/acme.key'
          certKeyPath: 'path/to/cert.key'

          # OPTIONAL
          reason: 'KeyCompromise'
```