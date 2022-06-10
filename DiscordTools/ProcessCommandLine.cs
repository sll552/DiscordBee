// taken from https://github.com/sonicmouse/ProcCmdLine/blob/524a662d1466c7342f54bcecfcd4a687005e573a/ManagedProcessCommandLine/ProcessCommandLine.cs
namespace MusicBeePlugin.DiscordTools
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Diagnostics;
  using System.Linq;
  using System.Runtime.InteropServices;

  public static class ProcessCommandLine
  {
    private static class Win32Native
    {
      public const uint PROCESS_BASIC_INFORMATION = 0;

      [Flags]
      public enum OpenProcessDesiredAccessFlags : uint
      {
        None = 0,
        PROCESS_VM_READ = 0x0010,
        PROCESS_QUERY_INFORMATION = 0x0400,
      }

      [StructLayout(LayoutKind.Sequential)]
      public struct ProcessBasicInformation
      {
        public IntPtr Reserved1;
        public IntPtr PebBaseAddress;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public IntPtr[] Reserved2;
        public IntPtr UniqueProcessId;
        public IntPtr Reserved3;
      }

      [StructLayout(LayoutKind.Sequential)]
      public struct UnicodeString
      {
        public ushort Length;
        public ushort MaximumLength;
        public IntPtr Buffer;
      }

      // This is not the real struct!
      // I faked it to get ProcessParameters address.
      // Actual struct definition:
      // https://docs.microsoft.com/en-us/windows/win32/api/winternl/ns-winternl-peb
      [StructLayout(LayoutKind.Sequential)]
      public struct PEB
      {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public IntPtr[] Reserved;
        public IntPtr ProcessParameters;
      }

      [StructLayout(LayoutKind.Sequential)]
      public struct RtlUserProcessParameters
      {
        public uint MaximumLength;
        public uint Length;
        public uint Flags;
        public uint DebugFlags;
        public IntPtr ConsoleHandle;
        public uint ConsoleFlags;
        public IntPtr StandardInput;
        public IntPtr StandardOutput;
        public IntPtr StandardError;
        public UnicodeString CurrentDirectory;
        public IntPtr CurrentDirectoryHandle;
        public UnicodeString DllPath;
        public UnicodeString ImagePathName;
        public UnicodeString CommandLine;
      }

      [DllImport("ntdll.dll")]
      public static extern uint NtQueryInformationProcess(
        IntPtr ProcessHandle,
        uint ProcessInformationClass,
        IntPtr ProcessInformation,
        uint ProcessInformationLength,
        out uint ReturnLength);

      [DllImport("kernel32.dll")]
      public static extern IntPtr OpenProcess(
        OpenProcessDesiredAccessFlags dwDesiredAccess,
        [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
        uint dwProcessId);

      [DllImport("kernel32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool ReadProcessMemory(
        IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer,
        uint nSize, out uint lpNumberOfBytesRead);

      [DllImport("kernel32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CloseHandle(IntPtr hObject);

      [DllImport("shell32.dll", SetLastError = true,
        CharSet = CharSet.Unicode, EntryPoint = "CommandLineToArgvW")]
      public static extern IntPtr CommandLineToArgv(string lpCmdLine, out int pNumArgs);
    }

    private static bool ReadStructFromProcessMemory<TStruct>(
      IntPtr hProcess, IntPtr lpBaseAddress, out TStruct val)
    {
      val = default;
      var structSize = Marshal.SizeOf<TStruct>();
      var mem = Marshal.AllocHGlobal(structSize);
      try
      {
        if (Win32Native.ReadProcessMemory(
          hProcess, lpBaseAddress, mem, (uint)structSize, out var len) &&
          (len == structSize))
        {
          val = Marshal.PtrToStructure<TStruct>(mem);
          return true;
        }
      }
      finally
      {
        Marshal.FreeHGlobal(mem);
      }
      return false;
    }

    public static string ErrorToString(int error) =>
      new string[]
      {
      "Success",
      "Failed to open process for reading",
      "Failed to query process information",
      "PEB address was null",
      "Failed to read PEB information",
      "Failed to read process parameters",
      "Failed to read parameter from process"
      }[Math.Abs(error)];

    public enum Parameter
    {
      CommandLine,
      WorkingDirectory,
    }

    public static int Retrieve(Process process, out string parameterValue, Parameter parameter = Parameter.CommandLine)
    {
      int rc = 0;
      parameterValue = null;
      var hProcess = Win32Native.OpenProcess(
        Win32Native.OpenProcessDesiredAccessFlags.PROCESS_QUERY_INFORMATION |
        Win32Native.OpenProcessDesiredAccessFlags.PROCESS_VM_READ, false, (uint)process.Id);
      if (hProcess != IntPtr.Zero)
      {
        try
        {
          var sizePBI = Marshal.SizeOf<Win32Native.ProcessBasicInformation>();
          var memPBI = Marshal.AllocHGlobal(sizePBI);
          try
          {
            var ret = Win32Native.NtQueryInformationProcess(
              hProcess, Win32Native.PROCESS_BASIC_INFORMATION, memPBI,
              (uint)sizePBI, out var len);
            if (ret == 0)
            {
              var pbiInfo = Marshal.PtrToStructure<Win32Native.ProcessBasicInformation>(memPBI);
              if (pbiInfo.PebBaseAddress != IntPtr.Zero)
              {
                if (ReadStructFromProcessMemory<Win32Native.PEB>(hProcess,
                  pbiInfo.PebBaseAddress, out var pebInfo))
                {
                  if (ReadStructFromProcessMemory<Win32Native.RtlUserProcessParameters>(
                    hProcess, pebInfo.ProcessParameters, out var ruppInfo))
                  {
                    string ReadUnicodeString(Win32Native.UnicodeString unicodeString)
                    {
                      var clLen = unicodeString.MaximumLength;
                      var memCL = Marshal.AllocHGlobal(clLen);
                      try
                      {
                        if (Win32Native.ReadProcessMemory(hProcess,
                          unicodeString.Buffer, memCL, clLen, out len))
                        {
                          rc = 0;
                          return Marshal.PtrToStringUni(memCL);
                        }
                        else
                        {
                          // couldn't read parameter line buffer
                          rc = -6;
                        }
                      }
                      finally
                      {
                        Marshal.FreeHGlobal(memCL);
                      }
                      return null;
                    }

                    switch (parameter)
                    {
                      case Parameter.CommandLine:
                        parameterValue = ReadUnicodeString(ruppInfo.CommandLine);
                        break;
                      case Parameter.WorkingDirectory:
                        parameterValue = ReadUnicodeString(ruppInfo.CurrentDirectory);
                        break;
                    }
                  }
                  else
                  {
                    // couldn't read ProcessParameters
                    rc = -5;
                  }
                }
                else
                {
                  // couldn't read PEB information
                  rc = -4;
                }
              }
              else
              {
                // PebBaseAddress is null
                rc = -3;
              }
            }
            else
            {
              // NtQueryInformationProcess failed
              rc = -2;
            }
          }
          finally
          {
            Marshal.FreeHGlobal(memPBI);
          }
        }
        finally
        {
          Win32Native.CloseHandle(hProcess);
        }
      }
      else
      {
        // couldn't open process for VM read
        rc = -1;
      }
      return rc;
    }

    public static IReadOnlyList<string> CommandLineToArgs(string commandLine)
    {
      if (string.IsNullOrEmpty(commandLine)) { return Array.Empty<string>(); }

      var argv = Win32Native.CommandLineToArgv(commandLine, out var argc);
      if (argv == IntPtr.Zero)
      {
        throw new Win32Exception(Marshal.GetLastWin32Error());
      }
      try
      {
        var args = new string[argc];
        for (var i = 0; i < args.Length; ++i)
        {
          var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
          args[i] = Marshal.PtrToStringUni(p);
        }
        return args.ToList().AsReadOnly();
      }
      finally
      {
        Marshal.FreeHGlobal(argv);
      }
    }
  }
}
