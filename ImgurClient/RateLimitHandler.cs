namespace MusicBeePlugin.ImgurClient
{
  using System.Linq;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;

  public class RateLimitHandler : DelegatingHandler
  {
    public const string UserRateLimitHeader = "X-RateLimit-UserRemaining";
    public const string AppRateLimitHeader = "X-RateLimit-ClientRemaining";
    private readonly string[] _rateLimitHeader = { UserRateLimitHeader, AppRateLimitHeader };
    // 20 because one upload uses 10 tokens
    public const int MinTokensLeft = 20;
    public bool IsRateLimited { get; private set; }

    public RateLimitHandler(HttpMessageHandler innerHandler) : base(innerHandler)
    {
      IsRateLimited = false;
    }

    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      var response = await base.SendAsync(request, cancellationToken);

      if (response != null)
      {
        foreach (var header in _rateLimitHeader)
        {
          if (response.Headers.Contains(header))
          {
            int tokensLeft = int.Parse(response.Headers.GetValues(header).First());
            IsRateLimited = tokensLeft <= MinTokensLeft;
            if (IsRateLimited)
            {
              break;
            }
          }
        }
      }

      return response;
    }
  }
}
