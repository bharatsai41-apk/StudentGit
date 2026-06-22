
namespace StudentGit;

using System;
using System.IO;
using Spectre.Console;

public class Program
{
    public static void Main(string[] args)
    {
        while (true)
        {
            RenderHeader();

            // Premium interactive selection grid
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[grey]Select an SDE Git command to simulate:[/]")
                    .PageSize(15) // Bumped up page size slightly to show everything cleanly
                    .MoreChoicesText("[grey](Move up and down to reveal more commands)[/]")
                    .AddChoiceGroup("Local Basics", new[] { "1. git status", "2. git init", "3. git add .", "4. git add [[file]]", "5. git commit", "6. git log" })
                    .AddChoiceGroup("Branch & Merge", new[] { "7. git branch (list)", "8. git branch (create)", "9. git switch", "10. git merge", "11. git stash" })
                    .AddChoiceGroup("Undo & Safety", new[] { "12. git restore [[file]]", "13. git reset [[file]]", "14. git revert [[hash]]", "15. git clean -f" })
                    .AddChoiceGroup("Remote Cloud", new[] { "16. git clone", "17. git remote add", "18. git fetch", "19. git pull", "20. git push" })
                    .AddChoiceGroup("Verification Test Suite", new[] { "99. Execute Automated Pipeline Test", "0. Exit Session" })); // FIXED: Added to explicit choice list

            if (choice.StartsWith("0"))
            {
                AnsiConsole.MarkupLine("[grey]Exiting StudentGit session. Goodbye![/]");
                break;
            }

            HandleCommand(choice);

            AnsiConsole.MarkupLine("\n[grey]Press any key to return to dashboard...[/]");
            Console.ReadKey(true);
        }
    }

    private static void RenderHeader()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[cyan]✦ StudentGit Engine v2.0[/]").LeftJustified().RuleStyle("grey"));
        AnsiConsole.MarkupLine("[grey]Dev Environment | SDE Pipeline Readiness[/]\n");
    }

    private static void HandleCommand(string choice)
    {
        GitResult result;

        int dotIndex = choice.IndexOf('.');
        string optionNumber = dotIndex >= 0 ? choice[..dotIndex].Trim() : choice.Trim();

        switch (optionNumber)
        {
            case "1":
            case "2":
                result = GitEngine.CheckHealthAndStage("");
                RenderOutput(result);
                break;

            case "3":
                result = GitEngine.CheckHealthAndStage(".");
                RenderOutput(result);
                break;

            case "4":
                string fileToStage = AnsiConsole.Ask<string>("[white]Enter exact filename to stage:[/] ");
                result = GitEngine.CheckHealthAndStage(fileToStage);
                RenderOutput(result);
                break;

            case "5":
                string cleanMsg = AnsiConsole.Prompt(
                    new TextPrompt<string>("[white]Enter commit message:[/]")
                        .Validate(input =>
                        {
                            string check = InputSanitizer.SanitizeCommitMessage(input);
                            return check switch
                            {
                                "ERROR_TOO_LONG_INPUT" => ValidationResult.Error("[red]Error: Input message is too long (Max 150 chars).[/]"),
                                "BLACKLIST_CHARACTER_ENTERED" => ValidationResult.Error("[red]Error: Found blacklisted characters (\", /, `).[/]"),
                                _ => ValidationResult.Success()
                            };
                        }));

                result = GitEngine.CommitChanges(InputSanitizer.SanitizeCommitMessage(cleanMsg));
                RenderOutput(result);
                break;

            case "6":
                result = GitEngine.GetCommitLog();
                RenderOutput(result);
                break;

            case "7":
                result = GitEngine.ListBranches();
                RenderOutput(result);
                break;

            case "8":
                string cleanBranch = AnsiConsole.Prompt(
                    new TextPrompt<string>("[white]Enter new branch name:[/]")
                        .Validate(input =>
                        {
                            string check = InputSanitizer.SanitizeBranchName(input);
                            return check switch
                            {
                                "STRING_IS_EMPTY" => ValidationResult.Error("[red]Error: Branch name cannot be empty.[/]"),
                                "ERROR_TOO_LONG_INPUT" => ValidationResult.Error("[red]Error: Name is too long (Max 50 chars).[/]"),
                                "BLACKLIST_CHARACTER_ENTERED" => ValidationResult.Error("[red]Error: Only letters, numbers, dashes, and underscores allowed.[/]"),
                                _ => ValidationResult.Success()
                            };
                        }));

                result = GitEngine.CreateBranch(InputSanitizer.SanitizeBranchName(cleanBranch));
                RenderOutput(result);
                break;

            case "9":
                string targetBranch = AnsiConsole.Ask<string>("[white]Enter target branch to switch to:[/] ");
                result = GitEngine.SwitchToBranch(targetBranch);
                RenderOutput(result);
                break;

            case "10":
                string sourceBranch = AnsiConsole.Ask<string>("[white]Enter source branch to merge:[/] ");
                result = GitEngine.MergeBranch(sourceBranch);
                RenderOutput(result);
                break;

            case "11":
                result = GitEngine.StashChanges();
                RenderOutput(result);
                break;

            case "12":
                string restorePath = AnsiConsole.Ask<string>("[white]Enter path of file to restore:[/] ");
                result = GitEngine.RestoreFile(restorePath);
                RenderOutput(result);
                break;

            case "13":
                string unstagePath = AnsiConsole.Ask<string>("[white]Enter path of file to unstage:[/] ");
                result = GitEngine.UnstageFile(unstagePath);
                RenderOutput(result);
                break;

            case "14":
                string hash = AnsiConsole.Ask<string>("[white]Enter target commit hash to revert:[/] ");
                result = GitEngine.RevertCommit(hash);
                RenderOutput(result);
                break;

            case "15":
                result = GitEngine.CleanRepository();
                RenderOutput(result);
                break;

            case "16":
            case "17":
                string cleanUrl = AnsiConsole.Prompt(
                    new TextPrompt<string>("[white]Enter GitHub remote repository URL:[/]")
                        .Validate(input =>
                        {
                            string check = InputSanitizer.SanitizeUrl(input);
                            return check switch
                            {
                                "STRING_IS_EMPTY" => ValidationResult.Error("[red]Error: URL cannot be empty.[/]"),
                                "ERROR_TOO_LONG_INPUT" => ValidationResult.Error("[red]Error: URL exceeds 120 character limit.[/]"),
                                "INVALID_URL" => ValidationResult.Error("[red]Error: Invalid format. Must be like https://github.com[/]"),
                                _ => ValidationResult.Success()
                            };
                        }));

                string sanitizedUrl = InputSanitizer.SanitizeUrl(cleanUrl);
                if (optionNumber == "16")
                {
                    string folder = AnsiConsole.Ask<string>("[white]Enter target local folder name:[/] ");
                    result = GitEngine.CloneRepository(sanitizedUrl, folder);
                }
                else
                {
                    result = GitEngine.AddRemote(sanitizedUrl);
                }
                RenderOutput(result);
                break;

            case "18":
                result = GitEngine.FetchUpdates();
                RenderOutput(result);
                break;

            case "19":
                result = GitEngine.PullUpdates();
                RenderOutput(result);
                break;

            case "20":
                result = GitEngine.PushChanges();
                RenderOutput(result);
                break;

            case "99": // FIXED: Directly accessible here
                RunAutomatedPipelineTest();
                break;

            default:
                AnsiConsole.MarkupLine("[red][!] Command unrecognised or not supported.[/]");
                break;
        }
    }

    private static void RenderOutput(GitResult result)
    {
        Console.WriteLine();
        if (result.IsSuccess)
        {
            AnsiConsole.MarkupLine($"[green]✓[/] {Markup.Escape(result.Message)}");
            if (!string.IsNullOrEmpty(result.StandardGitCommand))
            {
                AnsiConsole.MarkupLine($"  [grey]↳ Action:[/] [darkcyan]{Markup.Escape(result.StandardGitCommand.Replace("\n", " && "))}[/]");
            }
        }
        else
        {
            AnsiConsole.MarkupLine($"[yellow]⚠ Friction Alert:[/] {Markup.Escape(result.Message)}");
        }

        if (!string.IsNullOrEmpty(result.TrainerTip))
        {
            Console.WriteLine();
            var tipPanel = new Panel($"[purple]{Markup.Escape(result.TrainerTip)}[/]")
                .Header("[magenta]💡 Trainer Tip[/]")
                .BorderColor(Color.Magenta);

            AnsiConsole.Write(new Padder(tipPanel));
        }
    }


    private static void RunAutomatedPipelineTest()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]🧪 SDE Automated Integration Test[/]").LeftJustified());

        // FIXED: Force the test file path to point to the literal project root folder, bypassing bin/ filters
        string projectRoot = Directory.GetCurrentDirectory();
        string testFile = "SdeCoreApp.cs";
        string fullTestFilePath = Path.Combine(projectRoot, testFile);

        // Test 1: Check baseline status tracking
        AnsiConsole.MarkupLine("\n[blue][[Step 1/4]][/] Verifying repository health check...");
        var initResult = GitEngine.CheckHealthAndStage("");
        RenderOutput(initResult);

        // Test 2: Create a mock modified file on disk safely outside bin/
        AnsiConsole.MarkupLine($"\n[blue][[Step 2/4]][/] Generating uncommitted asset: {testFile}");
        File.WriteAllText(fullTestFilePath, "// Modern SDE C# Pipeline Code Base");
        var statusResult = GitEngine.CheckHealthAndStage("");
        RenderOutput(statusResult);

        // Test 3: Run Staging Engine
        AnsiConsole.MarkupLine($"\n[blue][[Step 3/4]][/] Attempting to stage {testFile}...");
        var stageResult = GitEngine.CheckHealthAndStage(testFile);
        RenderOutput(stageResult);

        // Test 4: Execute Commit Engine checkpoint
        AnsiConsole.MarkupLine("\n[blue][[Step 4/4]][/] Saving stable snapshot checkpoint...");
        var commitResult = GitEngine.CommitChanges("Feat: Implement SDE Core logic pipeline");
        RenderOutput(commitResult);

        // Test 5: Verify log outputs
        AnsiConsole.MarkupLine("\n[blue][[Verification]][/] Printing commit history logs:");
        var logResult = GitEngine.GetCommitLog();
        RenderOutput(logResult);

        // Clean up the generated test file so it doesn't leave clutter
        try
        {
            if (File.Exists(fullTestFilePath)) File.Delete(fullTestFilePath);
        }
        catch { /* Dust off clean up errors */ }
    }



}
