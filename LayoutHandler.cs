namespace MusicBeePlugin
{
  using System;
  using System.Collections.Generic;
  using System.Net;
  using System.Text.RegularExpressions;
  using System.Windows.Forms.VisualStyles;

  public class LayoutHandler
  {
    private readonly Regex _layoutElementRegex;

    /// <summary>
    /// Create a LayoutHandler instance
    /// </summary>
    /// <param name="layoutElementRegex">The Regex used to determine layout elements that will be replaced, the actual element name must be a subgroup e.g. '{(.+?)}'</param>
    public LayoutHandler(Regex layoutElementRegex)
    {
      _layoutElementRegex = layoutElementRegex ?? throw new ArgumentNullException(nameof(layoutElementRegex));
    }

    private string Clean(string input, Dictionary<string, string> values, string separators)
    {
      var emptyValues = new Dictionary<string, string>();

      foreach (var entry in values)
      {
        if (string.IsNullOrWhiteSpace(entry.Value))
        {
          emptyValues.Add(entry.Key, entry.Value);
        }
      }
      input = Replace(input, emptyValues);
      // remove excess whitespace
      input = Regex.Replace(input, "\\s+", " ");

      if (string.IsNullOrEmpty(separators))
      {
        return input;
      }

      foreach (var sep in separators)
      {
        var escsep = Regex.Escape(sep.ToString());
        // remove double separators
        input = Regex.Replace(input, "[" + escsep + "]{2,}", sep.ToString());
        // remove separator with whitespace in between
        input = Regex.Replace(input, "[" + escsep + "]\\s+[" + escsep + "]", sep.ToString());
        // remove separators and whitespace at the beginning
        input = Regex.Replace(input, "^\\s*[" + escsep + "]*\\s*", "");
        // remove separators and whitespace at the end
        input = Regex.Replace(input, "\\s*[" + escsep + "]+\\s*$", "");
      }

      return input;
    }

    private string Replace(string input, Dictionary<string, string> values)
    {
      foreach (Match match in _layoutElementRegex.Matches(input))
      {
        // complete match is group 0 so we need exactly 2 groups to be able to replace correctly
        if (match.Groups.Count != 2)
        {
          continue;
        }

        var key = match.Groups[1].Captures[0].Value;
        if (values.ContainsKey(key))
        {
          input = input.Replace(match.Value, values[key]);
        }
      }
      return input;
    }

    /// <summary>
    /// Render the layout elements in the given string using the values from the provided dictionary
    /// </summary>
    /// <param name="layoutStr">String to render</param>
    /// <param name="values">Value dictionary to use</param>
    /// <param name="separators">The separators used in this string, these will be used as character class in Regex</param>
    /// <returns>The rendered string</returns>
    public string Render(string layoutStr, Dictionary<string, string> values, string separators)
    {
      return Replace(Clean(layoutStr, values, separators), values);
    }

    /// <summary>
    /// Renders layout elements in the URL and URL encodes it.
    /// </summary>
    /// <param name="url">URL with layout elements</param>
    /// <param name="values">Value dictionary to use</param>
    /// <returns>The rendered URL as string</returns>
    public string RenderUrl(string url, Dictionary<string, string> values, char escapeCharacter)
    {
      var finalUrl = url;
      foreach (Match placeholder in _layoutElementRegex.Matches(url))
      {
        var render = UnEscapeUrl(WebUtility.UrlEncode(Render(placeholder.Value, values, "")), "###", escapeCharacter);
        finalUrl = finalUrl.Replace(placeholder.Value, render);
      }

      return finalUrl;
    }

    private string UnEscapeUrl(string url, string trigger, char escChar)
    {
      // The url must start with a trigger string to prevent unescaping where it's not wanted, e.g. artist names etc.
      // Virtual Tags that need to use this feature must start with this string for escaping to work.
      if (!url.StartsWith(WebUtility.UrlEncode(trigger)))
      {
        return url;
      }

      var finalUrl = url.Substring(WebUtility.UrlEncode(trigger).Length);
      var escCharEnc = WebUtility.UrlEncode(escChar.ToString());
      finalUrl = finalUrl.Replace(escCharEnc+escCharEnc, escChar.ToString());
      string[] split = finalUrl.Split(new string[] {escCharEnc}, StringSplitOptions.None);
      for (int i = 0; i < split.Length; i++)
      {
        if (i > 0 && split[i].StartsWith("%"))
        {
          var decoded = WebUtility.UrlDecode(split[i]);
          var firstCharEnc = WebUtility.UrlEncode(decoded[0].ToString());
          split[i] = decoded[0] + split[i].Substring(firstCharEnc.Length);
        }
      }

      finalUrl = string.Join(null, split);

      return finalUrl;
    }
  }
}
