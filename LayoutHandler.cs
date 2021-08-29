using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MusicBeePlugin
{
  class LayoutHandler
  {
    private readonly Regex _layoutElementRegex;

    /// <summary>
    /// Create a LayoutHandler instance
    /// </summary>
    /// <param name="layoutElementRegex">The Regex used to determine layout elemets that will be replaced, the actual element name must be a subgroup e.g. '{(.+?)}'</param>
    public LayoutHandler(Regex layoutElementRegex)
    {
      _layoutElementRegex = layoutElementRegex ?? throw new ArgumentNullException(nameof(layoutElementRegex));
    }

    private string Clean(string input, Dictionary<string, string> values, string seperators)
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

      if (string.IsNullOrEmpty(seperators)) return input;
      foreach (var sep in seperators)
      {
        var escsep = Regex.Escape(sep.ToString());
        // remove double seperators
        input = Regex.Replace(input, "[" + escsep + "]{2,}", sep.ToString());
        // remove seperator with whitespace in between
        input = Regex.Replace(input, "[" + escsep + "]\\s+[" + escsep + "]", sep.ToString());
        // remove seperators and whitespace at the beginning
        input = Regex.Replace(input, "^\\s*[" + escsep + "]*\\s*", "");
        // remove seperators and whitespace at the end
        input = Regex.Replace(input, "\\s*[" + escsep + "]+\\s*$", "");
      }

      return input;
    }

    private string Replace(string input, Dictionary<string, string> values)
    {
      var matches = _layoutElementRegex.Matches(input);
      foreach (Match match in matches)
      {
        // complete match is group 0 so we neeed exactly 2 groups to be able to replace correctly
        if (match.Groups.Count != 2) continue;
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
    /// <param name="values">Value disctionary to use</param>
    /// <param name="seperators">The seperators used in this string, these will be used as character class in Regex</param>
    /// <returns></returns>
    public string Render(string layoutStr, Dictionary<string, string> values, string seperators)
    {
      return Replace(Clean(layoutStr, values, seperators), values);
    }
  }
}
