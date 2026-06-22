namespace StudentGit;

using System;
using System.Diagnostics;

public static class GitEnvironmentManager
{
    public static bool IsGitInstalled()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "--version",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                process?.WaitForExit();
                return process?.ExitCode == 0;
            }
        }
        catch
        {
            return false;
        }
    }

    public static bool InstallGitSilently()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = "install --id Git.Git --silent --accept-source-agreements --accept-package-agreements",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                process?.WaitForExit();
                return process?.ExitCode == 0;
            }
        }
        catch
        {
            return false;
        }
    }
}
