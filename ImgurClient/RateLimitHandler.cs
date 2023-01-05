namespace MusicBeePlugin.ImgurClient
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;

  public class RateLimitHandler : DelegatingHandler
  {
    private readonly Dictionary<string, string> _rateLimitHeader = new Dictionary<string, string>() {
      { "X-RateLimit-UserRemaining", "IP is rate limited" },
      { "X-RateLimit-ClientRemaining", "Imgur app is rate limited" },
      { "X-Post-Rate-Limit-Remaining", "Post requests are rate limited" }
    };
    private string rateLimitInfo;
    // 20 because one upload uses 10 tokens
    public const int MinTokensLeft = 20;
    public bool IsRateLimited { get; private set; }
    public string RateLimitInfo => IsRateLimited ? rateLimitInfo : "";

    public RateLimitHandler(HttpMessageHandler innerHandler) : base(innerHandler)
    {
      IsRateLimited = false;
    }

    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      var response = await base.SendAsync(request, cancellationToken);

      if (response != null)
      {
        foreach (var header in _rateLimitHeader.Keys)
        {
          if (response.Headers.Contains(header))
          {
            int tokensLeft = int.Parse(response.Headers.GetValues(header).First());
            IsRateLimited = tokensLeft <= MinTokensLeft;
            if (IsRateLimited)
            {
              rateLimitInfo = _rateLimitHeader[header];
              break;
            }
          }
        }
      }

      return response;
    }
  }
}
