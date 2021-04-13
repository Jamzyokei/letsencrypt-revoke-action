FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-env

COPY . ./
RUN dotnet publish ./CinderBlockGames.GitHub.Actions.LERevoke/CinderBlockGames.GitHub.Actions.LERevoke.csproj -c Release -o out --no-self-contained

LABEL maintainer="cinder block games <hello@cinderblockgames.com>"
LABEL repository="https://github.com/cinderblockgames/letsencrypt-revoke-action"
LABEL homepage="https://cinderblockgames.com/"

LABEL com.github.actions.name="Revoke Certificate Issued by Let's Encrypt"
LABEL com.github.actions.description="This action uses either an account key or a private key to revoke a certificate issued by Let's Encrypt."
# https://docs.github.com/actions/creating-actions/metadata-syntax-for-github-actions#branding
LABEL com.github.actions.icon="lock"
LABEL com.github.actions.color="red"

FROM mcr.microsoft.com/dotnet/runtime:5.0
COPY --from=build-env /out .
ENTRYPOINT [ "dotnet", "/CinderBlockGames.GitHub.Actions.LERevoke.dll" ]