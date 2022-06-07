namespace MusicBeePlugin.ImgurClient
{
  using RestSharp;
  using RestSharp.Authenticators;
  using System.Threading.Tasks;

  internal class ImgurAuthenticator : AuthenticatorBase
  {
    public ImgurAuthenticator(string token) : base(token)
    {
    }

    protected override ValueTask<Parameter> GetAuthenticationParameter(string accessToken) => new ValueTask<Parameter>(new HeaderParameter(KnownHeaders.Authorization, $"Client-ID {Token}"));
  }
}
