namespace StudentGit;

using System;
using System.Text.RegularExpressions;

public class InputSanitizer
{
    private const int MaxCommitLength = 150;
    private const int MaxBranchLength = 50;
    private const int MaxUrlLength = 120;
    private const int MaxTokenLength = 100;

    public static string SanitizeCommitMessage(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return $"Progress snapshot {DateTime.Now:yyyy-MM-dd HH:mm}";
        }

        string clean = input.Trim();
        if (clean.Length > MaxCommitLength)
        {
            return "ERROR_TOO_LONG_INPUT";
        }

        string pattern = "^[^\"/`]+$";
        if (!Regex.IsMatch(clean, pattern))
        {
            return "BLACKLIST_CHARACTER_ENTERED";
        }

        return clean;
    }

    public static string SanitizeBranchName(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) // Fixed: Uniform naming casing
        {
            return "STRING_IS_EMPTY";
        }

        string clean = input.Trim();
        if (clean.Length > MaxBranchLength)
        {
            // Git branches cannot have spaces, convert them to dashes for the student
            clean = clean.Replace(" ", "-");
        }

        if (clean.Length > MaxBranchLength)
        {
            return "ERROR_TOO_LONG_INPUT";
        }

        string pattern = "^[A-Za-z0-9_-]+$";
        if (!Regex.IsMatch(clean, pattern))
        {
            return "BLACKLIST_CHARACTER_ENTERED";
        }

        return clean;
    }

    public static string SanitizeUrl(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return "STRING_IS_EMPTY";
        }

        string clean = input.Trim();
        if (clean.Length > MaxUrlLength)
        {
            return "ERROR_TOO_LONG_INPUT";
        }
        string pattern = "^https://github\\.com/[A-Za-z0-9-_]+/[A-Za-z0-9-_]+(\\.git)?$";
        if (!Regex.IsMatch(clean, pattern))
        {
            return "INVALID_URL";
        }

        return clean;
    }

    public static string SanitizeToken(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return "STRING_IS_EMPTY";
        }

        string clean = input.Trim();
        if (clean.Length > MaxTokenLength)
        {
            return "INVALID_INPUT";
        }

        bool isValidPrefix = clean.StartsWith("github_pat_") || clean.StartsWith("ghp_");
        string pattern = "^[A-Za-z0-9_]+$";
        string contentToTest = clean.Replace("github_pat_", "").Replace("ghp_", "");
        bool isValidExpression = Regex.IsMatch(contentToTest, pattern);

        if (!(isValidPrefix && isValidExpression))
        {
            return "INVALID_TOKEN_INPUT";
        }

        return clean;
    }
}
