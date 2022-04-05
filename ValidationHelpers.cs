namespace MusicBeePlugin
{
  using System;

  public static class ValidationHelpers
  {
    public static bool ValidateUri(string uri)
    {
      return Uri.TryCreate(uri, UriKind.Absolute, out Uri uriValidation)
             && (uriValidation.Scheme == Uri.UriSchemeHttp || uriValidation.Scheme == Uri.UriSchemeHttps);
    }
  }
}
