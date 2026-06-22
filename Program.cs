
namespace StudentGit;

using System.Diagnostics;
using System;
using System.IO;
using Spectre.Console;

public class Program
{
    public static void Main(string[] args)
    {
        if (!GitEnvironmentManager.IsGitInstalled())
        {
            AnsiConsole.MarkupLine("[yellow][!] Git environment dependency is missing from this system.[/]");

            bool installSuccess = AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("cyan"))
                .Start("[cyan]Auto-repairing environment: Running silent installation via winget...[/]", ctx =>
                {
                    return GitEnvironmentManager.InstallGitSilently();
                });

            if (installSuccess)
            {
                AnsiConsole.MarkupLine("[green][+] Git successfully installed. Initializing path structures...[/]");

                string oldPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine) ?? "";

                Environment.SetEnvironmentVariable("PATH", oldPath, EnvironmentVariableTarget.Process);
            }
            else
            {
                AnsiConsole.MarkupLine("[red][X] Error: Could not automatically resolve Git installation via winget.[/]");
                AnsiConsole.MarkupLine("[grey]Please manually install Git from git-scm.com and restart the tool.[/]");
                return;
            }
        }

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
        AnsiConsole.Write(new Rule("[yellow]🧪 SDE Comprehensive Master Test Matrix (All Phases)[/]").LeftJustified());

        string testFile = "SdeCoreApp.cs";
        string toxicFile = "broken_syntax_error.tmp";
        string stashFile = "experimental_feature.cs";
        string projectRoot = Directory.GetCurrentDirectory();
        string fullTestFilePath = Path.Combine(projectRoot, testFile);
        string fullToxicFilePath = Path.Combine(projectRoot, toxicFile);
        string fullStashFilePath = Path.Combine(projectRoot, stashFile);

        // =======================================================
        // METRIC 1: LOCAL WORKFLOW LOOP (Commands 1-6)
        // =======================================================
        AnsiConsole.Write(new Padder(new Rule("[cyan]1. Local Basics Workflow[/]").LeftJustified().RuleStyle("grey"), new Padding(0, 1, 0, 0)));

        AnsiConsole.MarkupLine("[blue][[Step 1/20]][/] Running baseline health check status...");
        RenderOutput(GitEngine.CheckHealthAndStage(""));

        AnsiConsole.MarkupLine("[blue][[Step 2/20]][/] Initializing repository checks...");
        RenderOutput(GitEngine.CheckHealthAndStage(""));

        AnsiConsole.MarkupLine($"[blue][[Step 3/20]][/] Writing fresh tracking asset: {testFile}");
        File.WriteAllText(fullTestFilePath, "// Core software architecture branch base");
        RenderOutput(GitEngine.CheckHealthAndStage(""));

        AnsiConsole.MarkupLine($"[blue][[Step 4/20]][/] Staging {testFile} to preparation index...");
        RenderOutput(GitEngine.CheckHealthAndStage(testFile));

        AnsiConsole.MarkupLine("[blue][[Step 5/20]][/] Committing staged changes to project history timeline...");
        RenderOutput(GitEngine.CommitChanges("Feat: Establish main framework base"));

        AnsiConsole.MarkupLine("[blue][[Step 6/20]][/] Verifying baseline commit logs...");
        RenderOutput(GitEngine.GetCommitLog());


        // =======================================================
        // METRIC 2: BRANCHING & ISOLATION SYSTEM (Commands 7-11)
        // =======================================================
        AnsiConsole.Write(new Padder(new Rule("[cyan]2. Branching & Context Switching[/]").LeftJustified().RuleStyle("grey"), new Padding(0, 1, 0, 0)));

        AnsiConsole.MarkupLine("[blue][[Step 7/20]][/] Spawning isolated branch: feature-login...");
        RenderOutput(GitEngine.CreateBranch("feature-login"));

        AnsiConsole.MarkupLine("[blue][[Step 8/20]][/] Querying branch index checklist matrix...");
        RenderOutput(GitEngine.ListBranches());

        AnsiConsole.MarkupLine("[blue][[Step 9/20]][/] Switching workspace context to branch: feature-login...");
        RenderOutput(GitEngine.SwitchToBranch("feature-login"));

        AnsiConsole.MarkupLine("[blue][[Step 10/20]][/] Writing code on branch and testing merge preview setup...");
        File.WriteAllText(fullStashFilePath, "// Branch experimental logic");
        RenderOutput(GitEngine.CheckHealthAndStage(stashFile));
        RenderOutput(GitEngine.CommitChanges("Feat: Add login layout"));

        // Switch back to master to prepare for merge simulation
        GitEngine.SwitchToBranch("master");
        AnsiConsole.MarkupLine("[blue][[Step 11]][/] Merging 'feature-login' cleanly back into master pipeline...");
        RenderOutput(GitEngine.MergeBranch("feature-login"));


        // =======================================================
        // METRIC 3: DISASTER RECOVERY & SAFETY NETS (Commands 12-15)
        // =======================================================
        AnsiConsole.Write(new Padder(new Rule("[cyan]3. Disaster Recovery & Safety Nets[/]").LeftJustified().RuleStyle("grey"), new Padding(0, 1, 0, 0)));

        AnsiConsole.MarkupLine($"[blue][[Step 12/20]][/] Simulating tracking pollution (Writing {toxicFile})...");
        File.WriteAllText(fullToxicFilePath, "toxic corrupted syntax code entry");
        RenderOutput(GitEngine.CheckHealthAndStage(""));

        AnsiConsole.MarkupLine("[blue][[Step 13/20]][/] Executing git clean routine to purge untracked disk files...");
        RenderOutput(GitEngine.CleanRepository());

        bool cleanPassed = !File.Exists(fullToxicFilePath);
        AnsiConsole.MarkupLine($"  [grey]↳ File removal assertion check:[/] {(cleanPassed ? "[green]SUCCESS (File Purged)[/]" : "[red]FAILED (File Leaked)[/]")}");

        // Track edit modification to check restoration tools
        if (File.Exists(fullTestFilePath))
        {
            File.AppendAllText(fullTestFilePath, "\n// Accidental broken edit string entry");

            AnsiConsole.MarkupLine($"\n[blue][[Step 14/20]][/] Staging broken file edits for reset simulation...");
            RenderOutput(GitEngine.CheckHealthAndStage(testFile));

            AnsiConsole.MarkupLine($"[blue][[Step 15/20]][/] Running git reset on {testFile} to unstage it...");
            RenderOutput(GitEngine.UnstageFile(testFile));

            AnsiConsole.MarkupLine($"[blue][[Step 16/20]][/] Restoring original code state via git restore...");
            RenderOutput(GitEngine.RestoreFile(testFile));
        }


        // =======================================================
        // METRIC 4: INTEGRATION & STASHING SHELVES (Advanced Lifecycle)
        // =======================================================
        AnsiConsole.Write(new Padder(new Rule("[cyan]4. Integration & Workspace Stashing[/]").LeftJustified().RuleStyle("grey"), new Padding(0, 1, 0, 0)));

        if (File.Exists(fullTestFilePath)) File.AppendAllText(fullTestFilePath, "\n// Half-finished experimental code block");
        AnsiConsole.MarkupLine("[blue][[Step 17/20]][/] Shelving uncommitted changes to temporary stash shelf...");
        RenderOutput(GitEngine.StashChanges());


        // =======================================================
        // METRIC 5: REMOTE MOCK NETWORK CLOUD LOOP (Commands 16-20)
        // =======================================================
        AnsiConsole.Write(new Padder(new Rule("[cyan]5. Remote Cloud Collaboration[/]").LeftJustified().RuleStyle("grey"), new Padding(0, 1, 0, 0)));

        AnsiConsole.MarkupLine("[blue][[Step 18/20]][/] Simulating target framework clone operation to backup directory...");
        string backupFolder = Path.Combine(projectRoot, "StudentGit_Backup");
        if (Directory.Exists(backupFolder)) Directory.Delete(backupFolder, true);
        RenderOutput(GitEngine.CloneRepository("https://github.com", backupFolder));

        AnsiConsole.MarkupLine("[blue][[Step 19/20]][/] Linking local repo up to a GitHub server endpoint URL...");
        try
        {
            using var repo = new LibGit2Sharp.Repository(projectRoot);
            if (repo.Network.Remotes["origin"] != null) repo.Network.Remotes.Remove("origin");
        }
        catch { }
        RenderOutput(GitEngine.AddRemote("https://github.com"));

        AnsiConsole.MarkupLine("\n[blue][[Step 20/20]][/] Pinging tracking servers to fetch upstream change metadata logs...");
        RenderOutput(GitEngine.FetchUpdates());

        // Final disk space hygiene cleanup routine
        try
        {
            if (File.Exists(fullTestFilePath)) File.Delete(fullTestFilePath);
            if (File.Exists(fullToxicFilePath)) File.Delete(fullToxicFilePath);
            if (File.Exists(fullStashFilePath)) File.Delete(fullStashFilePath);
            if (Directory.Exists(backupFolder)) Directory.Delete(backupFolder, true);
        }
        catch { }
    }
}
