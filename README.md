<!-- markdownlint-disable -->
# StudentGit Engine v2.0.1

[![Platform: Windows/Linux/macOS](https://shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-blue?style=flat-square)](https://github.com/bharatsai41-apk/StudentGit/releases)
[![Language: C#](https://shields.io/badge/language-C%23%2012-purple?style=flat-square)](https://dotnet.microsoft.com/)
[![Framework: .NET 10](https://shields.io/badge/framework-.NET%2010-512BD4?style=flat-square)](https://dotnet.microsoft.com/)
[![License: MIT](https://shields.io/badge/license-MIT-green?style=flat-square)](LICENSE)

> **An interactive, high-fidelity Software Development Engineer (SDE) Git simulation engine engineered for pipeline readiness testing.**

Powered by `LibGit2Sharp` and `Spectre.Console`, StudentGit allows developers to practice, simulate, and visually analyze complex Git commands and lifecycle workflows safely inside an isolated environment—perfect for **DevOps training, certification prep, and skill validation**.

---

## 🎯 Core Features

### ✨ Interactive Command Grid
- **20 Essential Git Commands** organized into 4 logical categories
- Premium terminal dashboard with organized choice routing panels
- Real-time command execution with detailed feedback

### 🧪 Automated 20/20 Test Matrix
- **Full-lifecycle automated validation** covering:
  - Local Basics (init, status, add, commit, log)
  - Branching & Context Switching (create, list, switch, merge, stash)
  - Disaster Recovery (restore, reset, revert, clean)
  - Integration & Shelving (git stash workflow)
  - Remote Cloud Collaboration (clone, fetch, pull, push)

### 🔐 PAT Token Authentication (v2.0.1 NEW)
- **Secure GitHub Personal Access Token (PAT) support**
- Masked token input for enhanced security
- Support for both public and private repository operations
- Token lifecycle management with `SetAuthToken()` and `ClearAuthToken()`

### ⚠️ Resilient Architecture
- Native error-trapping routines capture system resource locks and network issues
- Custom **"Friction Alert"** reporting dashboards for clear issue feedback
- Graceful fallback mechanisms for edge cases

### 🛡️ Rigorous Input Sanitization
- High-fidelity validation prevents blacklisted characters and input overflows
- Commit message length limits (150 chars max)
- Branch name validation (alphanumeric, dashes, underscores)
- URL format validation for GitHub repositories
- PAT format validation (github_pat_* or ghp_* prefix)

---

## 📦 Installation

### Quick Start (Download Binary)

1. **Download** the latest release for your operating system:
   - 🪟 [`StudentGit.exe`](https://github.com/bharatsai41-apk/StudentGit/releases) - Windows x64
   - 🐧 [`StudentGit`](https://github.com/bharatsai41-apk/StudentGit/releases) - Linux x64
   - 🍏 [`StudentGit`](https://github.com/bharatsai41-apk/StudentGit/releases) - macOS Arm64

2. **Extract** the executable to any folder

3. **Run** the application:
   ```bash
   ./StudentGit          # Linux/macOS
   StudentGit.exe        # Windows
   ```

> **ℹ️ Prerequisite:** .NET 10 Runtime required. [Download here](https://dotnet.microsoft.com/download/dotnet/10.0) if needed.

---

## 🌍 Global Path Configuration

### Windows (PowerShell)
```powershell
# Run as Administrator
[Environment]::SetEnvironmentVariable(
  "Path", 
  [Environment]::GetEnvironmentVariable("Path", "User") + ";C:\Path\To\StudentGit", 
  "User"
)
```
Then restart your terminal and run:
```cmd
StudentGit
```

### Linux
```bash
sudo cp StudentGit /usr/local/bin/StudentGit
sudo chmod +x /usr/local/bin/StudentGit
StudentGit
```

### macOS (Apple Silicon M1/M2/M3/M4)
```bash
sudo cp StudentGit /usr/local/bin/StudentGit
sudo chmod +x /usr/local/bin/StudentGit
sudo xattr -d com.apple.quarantine /usr/local/bin/StudentGit
StudentGit
```

---

## 🎮 Usage Guide

### Main Dashboard
Launch StudentGit to see the interactive command menu:

```
✦ StudentGit Engine v2.0.1
Dev Environment | SDE Pipeline Readiness

Select an SDE Git command to simulate:
  Local Basics
    1. git status
    2. git init
    3. git add .
    4. git add [file]
    5. git commit
    6. git log
  Branch & Merge
    7. git branch (list)
    8. git branch (create)
    9. git switch
    10. git merge
    11. git stash
  Undo & Safety
    12. git restore [file]
    13. git reset [file]
    14. git revert [hash]
    15. git clean -f
  Remote Cloud
    16. git clone
    17. git remote add
    18. git fetch
    19. git pull
    20. git push
  Verification Test Suite
    99. Execute Automated Pipeline Test
    0. Exit Session
```

### Command Categories

#### 🔹 **Local Basics** (1-6)
Perfect for learning Git fundamentals:
- Initialize repositories, check status, stage changes
- Create commits and review history

#### 🔹 **Branch & Merge** (7-11)
Master branching workflows:
- Create and switch between branches
- Merge feature branches safely
- Stash work in progress

#### 🔹 **Undo & Safety** (12-15)
Learn recovery techniques:
- Restore files to previous state
- Unstage changes without losing them
- Revert commits without erasing history
- Clean untracked files

#### 🔹 **Remote Cloud** (16-20)
Collaborate with remote repositories:
- Clone repositories (with optional PAT for private repos)
- Add remote origins
- **Fetch/Pull/Push with GitHub PAT authentication** ✨ NEW

#### 🔹 **Automated Test** (99)
Run the full 20-command validation pipeline automatically

---

## 🔐 Using GitHub PAT Authentication (v2.0.1 NEW)

### When You Need a PAT

You'll be prompted to provide a PAT when executing:
- `git clone` (for private repositories)
- `git fetch` (from private remotes)
- `git pull` (from private remotes)
- `git push` (to private remotes)

### How to Generate a PAT

1. Go to GitHub → **Settings** → **Developer settings** → **Personal access tokens**
2. Click **Generate new token** (or **Tokens (classic)**)
3. Select scopes: `repo` (full control of private repositories)
4. Copy the token (you won't see it again!)

### Using Your PAT in StudentGit

When prompted:
```
Do you need to use a Personal Access Token (PAT) for authentication? (Y/n): y
Enter your GitHub PAT token: [YOUR TOKEN - input is masked]
```

✅ Your token is:
- **Never displayed** on screen (masked input)
- **Never stored** on disk (only in memory)
- **Never logged** anywhere

---

## 📊 Test Suite Matrix (Option 99)

Running the **Automated Pipeline Test** validates all 20 commands in sequence:

| Phase | Commands | Focus |
|-------|----------|-------|
| **1. Local Basics** | 1-6 | Repository initialization and local workflow |
| **2. Branching** | 7-11 | Branch creation, switching, and merging |
| **3. Disaster Recovery** | 12-15 | File restoration, unstaging, and cleanup |
| **4. Integration** | 17 | Stashing work in progress |
| **5. Remote Cloud** | 16-20 | Clone, fetch, and push operations |

Each phase provides real-time feedback with:
- ✅ Success indicators for passed operations
- ⚠️ Friction Alerts for edge cases
- 💡 Trainer Tips explaining Git concepts

---

## 🛠️ Built With

| Component | Version | Purpose |
|-----------|---------|---------|
| **LibGit2Sharp** | 0.31.0 | Git operations and repository management |
| **Spectre.Console** | 0.57.0 | Rich terminal UI and interactive prompts |
| **.NET** | 10.0 | Runtime framework |

---

## 📋 System Requirements

- **OS:** Windows 10+, Linux (Ubuntu 20.04+), macOS 11+
- **RAM:** 256 MB minimum
- **Disk:** 50 MB (includes .NET runtime)
- **Runtime:** .NET 10 Runtime (auto-included with binaries)

---

## 🚀 What's New in v2.0.1

✨ **GitHub PAT Authentication**
- Secure token input with masked characters
- Support for private repository operations
- Token lifecycle management (SetAuthToken/ClearAuthToken)
- Validation for github_pat_* and ghp_* token formats

🔧 **Enhanced Remote Operations**
- Clone, Fetch, Pull, Push now support authentication
- Graceful error handling for permission issues
- Optional PAT prompts for public repositories

🎯 **Improved User Experience**
- Clear confirmation dialogs for PAT input
- Input validation with helpful error messages
- Non-intrusive UX (skip PAT if not needed)

---

## 🤝 Contributing

Found a bug or have a feature request? 
- [Open an issue](https://github.com/bharatsai41-apk/StudentGit/issues)
- Submit a pull request with improvements

---

## 📝 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

---

## 👨‍💼 Author

**Bharat Sai Mallina**
- GitHub: [@bharatsai41-apk](https://github.com/bharatsai41-apk)
- Purpose: Educational DevOps & SDE pipeline training tool

---

## 📚 Related Resources

- [Git Official Documentation](https://git-scm.com/doc)
- [GitHub CLI Documentation](https://cli.github.com/manual)
- [LibGit2Sharp Documentation](https://github.com/libgit2/libgit2sharp)
- [Spectre.Console Documentation](https://spectreconsole.net/)

---

**Happy learning! 🎓**

*StudentGit - Mastering Git, One Simulation at a Time.*
