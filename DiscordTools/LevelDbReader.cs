namespace MusicBeePlugin.DiscordTools
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;

  class LevelDbReader
  {
    private string _discordDir = null;
    private string _levelDbDir = null;
    private string _token = null;

    public string Token { get => _token; private set { } }

    public void Init()
    {
      if (detectDiscordDataDir())
      {
        readToken();
      }
    }

    private void readToken()
    {
      DirectoryInfo directoryRoot = new DirectoryInfo(_levelDbDir);

      foreach (var file in directoryRoot.GetFiles("*.ldb").OrderBy(f => f.LastWriteTime))
      {
        string fileOut = file.OpenText().ReadToEnd();
        Match mfaMatch = Regex.Match(fileOut, @"mfa\.[\w-]{84}");

        if (mfaMatch.Success)
        {
          _token = mfaMatch.Value;
          break;
        }
      }
    }

    private bool detectDiscordDataDir()
    {
      // Try to detect running discord processes and get the data dir from commandline args (this should work with all discord flavours e.g. canary)
      Process[] processCollection = Process.GetProcesses();
      foreach (var process in processCollection.Where(p => p.ProcessName.StartsWith("discord", StringComparison.OrdinalIgnoreCase)).ToList())
      {
        ProcessCommandLine.Retrieve(process, out string commandLine);
        _discordDir = ProcessCommandLine.CommandLineToArgs(commandLine).Where(arg => arg.IndexOf("user-data-dir", StringComparison.OrdinalIgnoreCase) >= 0).Select(arg => arg.Split('=').Last()).FirstOrDefault();
      }

      if (!Directory.Exists(_discordDir))
      {
        // We couldn't get the dir from the process so assume default dir
        _discordDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\discord";
      }
      _levelDbDir = $"{_discordDir}\\Local Storage\\leveldb\\";

      if (Directory.Exists(_levelDbDir))
      {
        Debug.WriteLine($"Got leveldb dir: {_levelDbDir}");
        return true;
      }
      else
      {
        Debug.WriteLine($"leveldb dir '{_levelDbDir}' does not exist");
        _levelDbDir = null;
        return false;
      }
    }
  }
}
