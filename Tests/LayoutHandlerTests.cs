namespace Tests
{
  using MusicBeePlugin;
  using System.Text.RegularExpressions;

  [TestClass]
  public class LayoutHandlerTests
  {
    private Dictionary<string, string> _placeholders = new()
    {
      {"Album", "..**!§$%&/(-.,asdfASDFöäü#+" },
      {"Artist", "Name Lastname" }
    };
    private LayoutHandler _layoutHandler;

    [TestInitialize]
    public void Setup()
    {
      _layoutHandler = new LayoutHandler(new Regex("\\[([^[]+?)\\]"));
    }

    [DataTestMethod]
    [DataRow("https://example.com/asdf/[Album]-[Artist]/[Test]/ffff", "asdf", '\\', "https://example.com/asdf/..**!%C2%A7%24%25%26%2F(-.%2CasdfASDF%C3%B6%C3%A4%C3%BC%23%2B-Name+Lastname/asdf/ffff")]
    [DataRow("https://example.com/asdf/[Album]-[Artist]/[Test]/ffff", "..**!§$%&/(-.,asdfASDFöäü#+", '\\', "https://example.com/asdf/..**!%C2%A7%24%25%26%2F(-.%2CasdfASDF%C3%B6%C3%A4%C3%BC%23%2B-Name+Lastname/..**!%C2%A7%24%25%26%2F(-.%2CasdfASDF%C3%B6%C3%A4%C3%BC%23%2B/ffff")]
    [DataRow("https://example.com/asdf/[Album]-[Artist]/[Test]/ffff", "###asdf/fff\\/dfg", '\\', "https://example.com/asdf/..**!%C2%A7%24%25%26%2F(-.%2CasdfASDF%C3%B6%C3%A4%C3%BC%23%2B-Name+Lastname/asdf%2Ffff/dfg/ffff")]
    [DataRow("https://example.com/asdf/[Album]-[Artist]/[Test]/ffff", "###asdf\\\\/fff\\/dfg", '\\', "https://example.com/asdf/..**!%C2%A7%24%25%26%2F(-.%2CasdfASDF%C3%B6%C3%A4%C3%BC%23%2B-Name+Lastname/asdf\\%2Ffff/dfg/ffff")]
    public void RenderUrl_Test(string url, string testString, char escapeCharacter, string expected)
    {
      _placeholders["Test"] = testString;

      var result = _layoutHandler.RenderUrl(url, _placeholders, escapeCharacter);

      Assert.AreEqual(expected, result);
    }
  }
}
