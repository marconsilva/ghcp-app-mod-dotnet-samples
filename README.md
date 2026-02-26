# .NET Application Modernization with GitHub Copilot

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-blue.svg)](https://dotnet.microsoft.com/)
[![Azure](https://img.shields.io/badge/Azure-Cloud%20Native-0078D4.svg)](https://azure.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

This repository demonstrates **end-to-end .NET application modernization** using the **GitHub Copilot App Modernization plugin**. It showcases a complete journey from legacy .NET Framework code to modern, containerized, cloud-native applications running on Azure.

## 📖 Table of Contents

- [Overview](#overview)
- [About Contoso University](#about-contoso-university)
- [Architecture Evolution](#architecture-evolution)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Solution Structure](#solution-structure)
- [Branch Structure](#branch-structure)
- [Plugin Installation](#plugin-installation)
- [Modernization Guide](#modernization-guide)
- [Screenshots](#screenshots)
- [Contributing](#contributing)

---

## 🎯 Overview

This repository serves as a **hands-on sample** for modernizing legacy .NET applications using AI-powered tools. The `main` branch contains the original legacy codebase, and each subsequent branch represents a stage in the modernization process, following the GitHub Copilot App Modernization plugin's workflow.

### Modernization Workflow Stages

```
Assess → Upgrade → Migrate → CVE Check → Build Fix → Unit Test → Containerize → Deploy
```

Each branch demonstrates:
- ✅ Specific modernization tasks completed
- 📝 Code changes and refactoring
- 🔧 Configuration updates
- 📚 Documentation and lessons learned

---

## 🎓 About Contoso University

**Contoso University** is a university management application that showcases common patterns found in enterprise .NET applications. It's an ideal candidate for modernization as it contains:

- Traditional ASP.NET MVC architecture
- Windows-specific dependencies (MSMQ, local file system)
- .NET Framework 4.8 codebase
- Entity Framework for data access
- Legacy deployment patterns

### Current State (Legacy - `main` branch)

| Component | Technology |
|-----------|------------|
| **Framework** | .NET Framework 4.8 |
| **Web Framework** | ASP.NET MVC 5 |
| **Database** | SQL Server LocalDB |
| **ORM** | Entity Framework Core 3.1.32 |
| **Messaging** | MSMQ (Microsoft Message Queue) |
| **File Storage** | Local File System |
| **Hosting** | IIS on Windows Server |

### Target State (Modernized)

| Component | Technology |
|-----------|------------|
| **Framework** | .NET 9+ |
| **Web Framework** | ASP.NET Core MVC |
| **Database** | Azure SQL Database |
| **ORM** | Entity Framework Core (latest) |
| **Messaging** | Azure Service Bus |
| **File Storage** | Azure Blob Storage |
| **Hosting** | Azure Container Apps |
| **Containerization** | Docker |

---

## 🚀 Features

Contoso University includes comprehensive university management capabilities:

### Core Functionality
- 👨‍🎓 **Student Management**: Full CRUD operations with search and pagination
- 📚 **Course Management**: Course creation, assignment, and tracking
- 👩‍🏫 **Instructor Management**: Instructor profiles and office assignments
- 🏢 **Department Management**: Department administration and course associations
- 📊 **Enrollment Statistics**: Visual enrollment data by date

### Technical Features (Added for Modernization Demo)
- 🔔 **Notification System**: Message queue-based notifications (MSMQ → Azure Service Bus)
- 📤 **File Upload System**: Teaching material uploads (File System → Azure Blob Storage)
- 🔐 **Authentication**: Windows Authentication → Azure AD B2C (in modernized version)

---

## ✅ Prerequisites

### For Running the Legacy Application (main branch)

- **Operating System**: Windows 10/11 or Windows Server
- **IDE**: Visual Studio 2019/2022 (any edition)
- **Runtime**: .NET Framework 4.8 SDK
- **Database**: SQL Server 2016+ or SQL Server LocalDB
- **MSMQ**: Microsoft Message Queuing feature enabled
  ```powershell
  # Enable MSMQ on Windows
  Enable-WindowsOptionalFeature -Online -FeatureName MSMQ-Server -All
  ```

### For Modernization (working with branches)

- **IDE**: 
  - Visual Studio Code with extensions (see [Plugin Installation](#plugin-installation))
  - OR Visual Studio 2022 (v17.8+) with GitHub Copilot
- **Runtime**: .NET 9 SDK or later
- **Docker**: Docker Desktop for Windows
- **Azure Account**: Free or paid Azure subscription
- **Azure CLI**: For deployment tasks
- **Git**: For branch navigation

---

## 🏃 Getting Started

### Running the Legacy Application

1. **Clone the repository**
   ```bash
   git clone https://github.com/marconsilva/ghcp-app-mod-dotnet-samples.git
   cd ghcp-app-mod-dotnet-samples
   ```

2. **Ensure MSMQ is enabled** (Windows feature)
   ```powershell
   Get-WindowsOptionalFeature -Online -FeatureName MSMQ-Server
   ```

3. **Open the solution**
   ```bash
   cd ContosoUniversity
   start ContosoUniversity.sln
   ```

4. **Restore NuGet packages**
   - In Visual Studio: Right-click solution → "Restore NuGet Packages"
   - Or via command line:
     ```bash
     nuget restore ContosoUniversity.sln
     ```

5. **Build the solution**
   - Press `Ctrl+Shift+B` or use Build → Build Solution

6. **Run the application**
   - Press `F5` to run with debugging
   - The application will launch in IIS Express
   - Default URL: `http://localhost:5000` or `https://localhost:5001`

7. **Initialize the database**
   - On first run, the application will create and seed the database
   - Sample data includes students, instructors, courses, and departments

### Exploring Modernization Branches

To see a specific modernization stage:

```bash
# List all branches
git branch -a

# Switch to a specific branch (e.g., assess branch)
git checkout assess

# Or view the progression
git checkout upgrade
git checkout migrate
git checkout cve-check
# ... and so on
```

---

## 📁 Solution Structure

```
dotnet-migration-copilot-samples/
├── README.md                          # This file
├── LICENSE
└── ContosoUniversity/                 # Main application folder
    ├── ContosoUniversity.sln          # Visual Studio solution
    ├── ContosoUniversity.csproj       # Project file
    ├── Web.config                     # Configuration
    ├── Global.asax                    # Application entry point
    ├── packages.config                # NuGet package manifest
    │
    ├── App_Start/                     # Application startup configuration
    │   ├── BundleConfig.cs           # CSS/JS bundling
    │   ├── FilterConfig.cs           # Global filters
    │   └── RouteConfig.cs            # MVC routing
    │
    ├── Controllers/                   # MVC Controllers
    │   ├── HomeController.cs         # Home page
    │   ├── StudentsController.cs     # Student management
    │   ├── CoursesController.cs      # Course management
    │   ├── InstructorsController.cs  # Instructor management
    │   ├── DepartmentsController.cs  # Department management
    │   └── NotificationsController.cs # Notification system
    │
    ├── Models/                        # Domain models
    │   ├── Student.cs
    │   ├── Course.cs
    │   ├── Instructor.cs
    │   ├── Department.cs
    │   ├── Enrollment.cs
    │   └── Notification.cs
    │
    ├── Data/                          # Data access layer
    │   └── SchoolContext.cs          # EF DbContext
    │   └── DbInitializer.cs          # Database seeding
    │
    ├── Views/                         # Razor views
    │   ├── Home/
    │   ├── Students/
    │   ├── Courses/
    │   ├── Instructors/
    │   ├── Departments/
    │   ├── Notifications/
    │   └── Shared/
    │
    ├── Services/                      # Business logic services
    │   └── NotificationService.cs
    │
    ├── Content/                       # Static CSS
    ├── Scripts/                       # JavaScript files
    ├── Uploads/                       # File upload directory
    └── Properties/                    # Assembly info
```

---

## 🌳 Branch Structure

This repository uses a **branch-per-stage** approach to demonstrate the modernization journey:

| Branch | Stage | Description |
|--------|-------|-------------|
| `main` | **Legacy Application** | Original .NET Framework 4.8 codebase - starting point |
| `upgrade-to-NET9-assess` | **Assessment** | Analysis results, compatibility reports, and modernization recommendations |
| `upgrade-to-NET9-upgrade` | **Framework Upgrade** | Upgrade to .NET 9, project file modernization (SDK-style) |
| `upgrade-to-NET9-cve-check` | **Security Scan** | Vulnerability assessment and package updates |
| `upgrade-to-NET9-unit-test` | **Testing** | Unit test implementation and test coverage |
| `upgrade-to-NET9-containerize` | **Containerization** | Dockerfile, container optimization, multi-stage builds 

### How to Use the Branches

1. **Start with `main`**: Understand the legacy application
2. **Review `assess`**: See the assessment report and plan
3. **Follow sequentially**: Each branch builds on the previous one
4. **Compare changes**: Use `git diff` to see what changed between stages
   ```bash
   git diff main..assess
   git diff assess..upgrade
   ```
5. **Learn from commits**: Each commit message explains the "why" behind changes

---

## 🔌 Plugin Installation

### Visual Studio Code

1. **Install Visual Studio Code**
   - Download from [code.visualstudio.com](https://code.visualstudio.com/)

2. **Install Required Extensions**

   Open VS Code and install these extensions:

   ```bash
   # GitHub Copilot
   code --install-extension GitHub.copilot
   
   # GitHub Copilot Chat
   code --install-extension GitHub.copilot-chat
   
   # C# Dev Kit
   code --install-extension ms-dotnettools.csdevkit
   
   # Azure Tools (for deployment)
   code --install-extension ms-vscode.vscode-node-azure-pack
   ```

   **Or** install via UI:
   - Click Extensions icon (`Ctrl+Shift+X`)
   - Search for and install:
     - ✅ **GitHub Copilot** 
     - ✅ **GitHub Copilot Chat**
     - ✅ **C# Dev Kit**
     - ✅ **Azure Tools**

3. **Sign in to GitHub Copilot**
   - Press `Ctrl+Shift+P`
   - Type "GitHub Copilot: Sign In"
   - Follow authentication prompts

4. **Install App Modernization Plugin**
   
   The GitHub Copilot App Modernization plugin provides specialized assistance for .NET migration:

   - Open **Copilot Chat** (`Ctrl+Alt+I`)
   - Type: `@workspace /help`
   - The plugin should be auto-detected if working with .NET Framework projects
   - Or install from: VS Code Extensions → Search "GitHub Copilot for App Modernization"

### Visual Studio 2022

1. **Install Visual Studio 2022** (v17.8 or later)
   - Download from [visualstudio.com](https://visualstudio.microsoft.com/)
   - Ensure ".NET desktop development" workload is installed

2. **Install GitHub Copilot Extension**
   
   **Method 1: Via Extensions Manager**
   - Open Visual Studio 2022
   - Go to **Extensions** → **Manage Extensions**
   - Click **Online** tab
   - Search for "**GitHub Copilot**"
   - Click **Download** and restart Visual Studio
   
   **Method 2: Via Visual Studio Installer**
   - Open **Visual Studio Installer**
   - Click **Modify** on your VS 2022 installation
   - Go to **Individual Components**
   - Search for "GitHub Copilot"
   - Check the box and click **Modify**

3. **Sign in to GitHub Copilot**
   - Restart Visual Studio
   - Go to **Tools** → **Options** → **GitHub** → **Copilot**
   - Click **Sign In**
   - Authenticate with your GitHub account

4. **Use App Modernization Features**
   
   - Open your .NET Framework solution
   - Right-click on the solution or project
   - Look for **Copilot** options in the context menu
   - Use **Copilot Chat** window (`View` → `GitHub Copilot Chat`)
   - Ask modernization questions like:
     ```
     How do I migrate this project to .NET 8?
     What are the breaking changes in my code?
     Help me containerize this application
     ```

### Verifying Installation

**VS Code:**
```bash
# In VS Code terminal
dotnet --version   # Should show .NET 8+
docker --version   # Should show Docker version
```

**Visual Studio 2022:**
- Look for the Copilot icon in the status bar (bottom-right)
- Open **View** → **GitHub Copilot Chat**
- Type a question and verify Copilot responds

---

## 📚 Modernization Guide

This comprehensive guide walks you through each stage of modernizing the Contoso University application using the **GitHub Copilot App Modernization Plugin**. Each stage builds upon the previous one, gradually transforming the legacy .NET Framework application into a modern, cloud-native solution.

> **💡 Important**: Work through these stages sequentially. Each stage has dependencies on the previous stages' outputs and configurations.

---

### 🔍 Stage 1: Assessment & Planning

**Objective**: Analyze the current application architecture, identify modernization opportunities, and create a comprehensive upgrade plan.

**Why This Matters**: A thorough assessment prevents costly mistakes by identifying potential blockers, compatibility issues, and required code changes before you begin the actual migration work.

#### Step-by-Step Instructions

**1.1 Open Your Solution**
   - Launch **Visual Studio 2022**
   - Open `ContosoUniversity.sln` from the repository
   - Ensure you're on the `main` branch (check the Git branch indicator in the status bar)

**1.2 Initiate the Modernization Workflow**
   - In **Solution Explorer**, right-click on the solution name (not a project)
   - Select **"Modernize"** from the context menu to open the GitHub Copilot modernization chat window
   
   ![Opening the modernization chat](/img/mod_chat_rightclick.png)

**1.3 Start the Assessment**
   - In the modernization chat window, select **"Upgrade to a newer version of .NET"**
   - This initiates an AI-powered analysis of your entire solution
   
   ![Starting the modernization process](/img/mod_chat_start.png)

**1.4 Review the Generated Upgrade Plan**
   - Navigate to `.github/upgrades/` folder in Solution Explorer
   - Open the generated `dotnet-upgrade-plan.md` file
   - This document contains:
     - 📊 Current state analysis
     - 🎯 Recommended target framework (.NET 8 or .NET 9)
     - ⚠️ Potential breaking changes
     - 📦 Package compatibility report
     - 🔄 Migration strategy with phases
     - ⏱️ Estimated effort and complexity

**1.5 Customize the Plan (Optional but Recommended)**
   - Review Copilot's recommendations carefully
   - If you want to modify the target framework version, ask in the chat:
     ```
     I want to target .NET 9 instead of .NET 8. Please update the upgrade plan accordingly.
     ```
   - You can also request specific changes:
     ```
     Include a plan for migrating from MSMQ to Azure Service Bus
     Add assessment for file system to Azure Blob Storage migration
     ```

**1.6 Save Your Assessment**
   - Consider creating a new branch for your modernization work:
     ```bash
     git checkout -b upgrade-to-NET9-assess
     git add .github/upgrades/
     git commit -m "Add .NET 9 upgrade assessment and plan"
     ```

> **⚠️ Critical**: Read both the chat window output AND the generated markdown file thoroughly. The chat provides contextual explanations, while the markdown file serves as your roadmap for all subsequent stages. Understanding the "why" behind each recommendation is crucial for making informed decisions during the upgrade.

> **💡 Pro Tip**: If this is your first .NET modernization, read the entire plan before proceeding. Highlight sections that might affect your application's critical functionality.

**Expected Outcomes**:
- ✅ Complete assessment report
- ✅ Documented upgrade strategy
- ✅ List of dependencies requiring updates
- ✅ Risk mitigation strategies identified
- ✅ Understanding of the work ahead

---

### ⬆️ Stage 2: Framework Upgrade

**Objective**: Migrate the project from .NET Framework 4.8 to .NET 9, converting to SDK-style project format and resolving initial compatibility issues.

**Why This Matters**: The framework upgrade is the foundation of modernization. It unlocks performance improvements, modern language features, cross-platform capabilities, and prepares your application for containerization and cloud deployment.

#### Step-by-Step Instructions

**2.1 Ensure Assessment is Complete**
   - Verify you have reviewed the upgrade plan from Stage 1
   - Make sure you understand which packages need updating
   - Have the `dotnet-upgrade-plan.md` file accessible for reference

**2.2 Initiate the Upgrade Process**
   - In the GitHub Copilot chat window, type:
     ```
     Continue with upgrade to .NET 9
     ```
   - Copilot will begin implementing the changes outlined in your plan
   - A progress tracking window will appear showing each stage of the upgrade

**2.3 Monitor Progress**
   - Watch the progress window in the Visual Studio main editor area
   - The window displays:
     - ✅ Completed steps (in green)
     - 🔄 Current step being processed
     - ⏳ Pending steps
     - ⚠️ Issues requiring investigation
   
   ![Tracking upgrade progress](/img/mod_chat_upgrade_iterate.png)

**2.4 Handle Command Execution Prompts**
   - Copilot will request permission to execute various commands (dotnet CLI commands, file operations, etc.)
   - **Options for approving commands**:
     - **"Allow Once"**: Approve individual commands (safest, but more clicks)
     - **"Allow in this Session"**: Auto-approve for the current VS session
     - **"Allow Always"**: Auto-approve for future sessions (use with caution)
   
   ![Allowing command execution](/img/mod_chat_upgrade_iterate_allowAlways.png)

   > **⚠️ Security Note**: Always read the command before approving. Look for operations that modify critical files, install packages, or make system changes. Use "Allow Always" only if you trust the source and understand the implications.

**2.5 Respond to Copilot's Questions**
   - During the upgrade, Copilot may pause to ask for your input on:
     - Package version conflicts
     - Breaking API changes requiring decisions
     - Alternative approaches for deprecated features
     - Configuration preferences
   - Provide clear, specific answers in the chat window
   - If unsure, ask Copilot for recommendations:
     ```
     What would you recommend for [specific issue]?
     What are the pros and cons of each approach?
     ```

**2.6 Iteratively Resolve Issues**
   - The progress window will highlight issues marked for "Investigation"
   - For each issue:
     1. Review the error message or warning
     2. Ask Copilot for resolution strategies
     3. Approve the suggested fix
     4. Wait for Copilot to implement and test
     5. Move to the next issue
   
   ![Completed upgrade progress](/img/mod_chat_upgrade_iterate_Progress_done.png)

   - Continue this cycle until all issues are resolved
   - This may require several rounds of iteration—be patient!

**2.7 Verify the Upgrade**
   - Once Copilot reports completion, perform these verification steps:
   
   **Build Verification**:
   ```bash
   # Clean and rebuild the solution
   dotnet clean
   dotnet build
   ```
   
   **Configuration Check**:
   - Open the `.csproj` file and verify:
     ```xml
     <Project Sdk="Microsoft.NET.Sdk.Web">
       <PropertyGroup>
         <TargetFramework>net9.0</TargetFramework>
       </PropertyGroup>
     </Project>
     ```

**2.8 Run the Application**
   - Press **F5** to start debugging
   - Test core functionality:
     - ✅ Application launches without errors
     - ✅ Home page loads
     - ✅ Database connection works
     - ✅ Student, Course, Instructor, and Department pages function correctly
     - ✅ Navigation works as expected

**2.9 Troubleshoot Runtime Issues**
   - If you encounter errors during runtime:
     1. **Copy the complete error message** (including stack trace)
     2. **Return to Copilot chat** and paste the error:
        ```
        I'm getting this runtime error after the upgrade:
        [paste full error message and stack trace]
        
        Please help me diagnose and fix this issue.
        ```
     3. Follow Copilot's debugging recommendations
     4. Test the fix and iterate if needed

**2.10 Save Your Progress**
   ```bash
   # Create a branch for this stage
   git checkout -b upgrade-to-NET9-upgrade
   git add .
   git commit -m "Complete framework upgrade to .NET 9"
   git push origin upgrade-to-NET9-upgrade
   ```

> **💡 Pro Tip**: Take screenshots of any unique issues you encounter and their resolutions. This creates valuable documentation for your team and future projects.

> **📝 Expected Migration Changes**: Copilot will typically convert your project file to SDK-style format, update package references to .NET 9 compatible versions, replace `Web.config` with `appsettings.json`, create `Program.cs` with minimal hosting model, and refactor startup configuration logic.

**Expected Outcomes**:
- ✅ Project converted to SDK-style format
- ✅ Target framework set to .NET 9
- ✅ All packages updated to compatible versions
- ✅ Application compiles without errors
- ✅ Application runs and core features work

---

### 🔒 Stage 3: CVE Check & Security Vulnerability Assessment

**Objective**: Identify and remediate security vulnerabilities in your dependencies, ensuring your modernized application meets current security standards.

**Why This Matters**: Legacy applications often include outdated packages with known security vulnerabilities (CVEs). Modernization is the perfect opportunity to eliminate these risks before deploying to production or the cloud.

#### Step-by-Step Instructions

**3.1 Initiate Security Scan**
   - With your upgraded .NET 9 solution open, return to the GitHub Copilot chat window
   - Request a comprehensive security assessment:
     ```
     Perform a CVE check on my project and run a comprehensive vulnerability assessment
     ```
   - Copilot will scan all NuGet packages and dependencies for known vulnerabilities

**3.2 Understand the Scan Results**
   - Navigate to the generated `vulnerability-assessment.md` file (typically in `.github/security/` or `.github/upgrades/`)
   - The report includes:
     - 🔴 **Critical vulnerabilities**: Immediate action required
     - 🟠 **High severity**: Address before production
     - 🟡 **Medium severity**: Plan remediation
     - 🟢 **Low severity**: Address as time permits
     - 📊 Overall security score

**3.3 Review Vulnerable Packages**
   - The report will list each vulnerable package with:
     - Package name and current version
     - CVE identifiers (e.g., CVE-2024-12345)
     - Severity level and CVSS score
     - Description of the vulnerability
     - Recommended version to upgrade to
     - Whether the vulnerability affects your code

   **Example Entry**:
   ```markdown
   ### System.Text.Json (Current: 7.0.0)
   - CVE: CVE-2024-30105
   - Severity: HIGH
   - CVSS Score: 7.5
   - Issue: Denial of service via malformed JSON
   - Recommendation: Upgrade to 8.0.1 or later
   - Status: Direct dependency - REQUIRES ACTION
   ```

**3.4 Apply Package Updates**
   - Copilot will typically provide update commands or make the changes automatically, but if he needs your command to perform the changes tell him to procceed with the updates as you have already reviewed the vulnerabilities and their impact now.

**3.5 Verify Updates Don't Break Functionality**
   - After applying updates:
     ```bash
     dotnet restore
     dotnet build
     ```
   - Run the application (F5) and test critical paths
   - Pay special attention to features using updated packages


**3.6 Implement Security Best Practices**
   - Copilot may recommend additional security enhancements:
     - Enable HTTPS redirection
     - Configure security headers
     - Implement CORS policies
     - Add authentication/authorization middleware
     - Enable request validation
   
   - Ask for specific guidance:
     ```
     What security best practices should I implement for a production-ready .NET 9 web application?
     ```

**3.7 Document Security Posture**
   - Save the final vulnerability assessment report
   - Document any accepted risks (if unable to update certain packages)
   - Create a tracking issue for any medium/low severity items to address later

**3.8 Commit Security Improvements**
   ```bash
   git checkout -b upgrade-to-NET9-cve-check
   git add .
   git commit -m "Resolve security vulnerabilities and update packages"
   git push origin upgrade-to-NET9-cve-check
   ```

> **⚠️ Important**: Some package updates may introduce breaking API changes. Always test thoroughly after applying security updates, especially for major version bumps.

> **💡 Pro Tip**: Set up automated dependency scanning in your CI/CD pipeline (GitHub Dependabot, Azure Pipelines security scanning) to catch vulnerabilities early in future development.

> **🔍 Alternative Tools**: While Copilot provides excellent guidance, you can also use `dotnet list package --vulnerable` command for a quick CLI-based scan, or integrate tools like OWASP Dependency-Check into your build process.

**Expected Outcomes**:
- ✅ Complete vulnerability assessment report
- ✅ All critical and high-severity CVEs resolved
- ✅ Documentation of security posture
- ✅ Application still functions correctly after updates
- ✅ Security best practices implemented

---

### 🧪 Stage 4: Unit Testing & Quality Assurance

**Objective**: Establish comprehensive test coverage to validate functionality, prevent regressions, and ensure the modernized application behaves identically to the legacy version.

**Why This Matters**: Modernization introduces risk. Unit tests act as a safety net, catching breaking changes before they reach production. They also serve as living documentation of expected behavior and enable confident refactoring.

#### Step-by-Step Instructions

**4.1 Plan Your Testing Strategy**
   - With your solution open, ask Copilot to develop a comprehensive test plan:
     ```
     Create a comprehensive plan to build unit tests that cover all critical aspects of the application, particularly areas affected by the modernization. Include tests for controllers, services, data access, and business logic. Create these tests under a new 'tests' folder with proper structure.
     ```

**4.2 Review the Test Implementation Plan**
   - Copilot will generate a test strategy document (`test-implementation-plan.md` or similar)
   - The plan typically includes:
     - 📋 Test project structure
     - 🎯 Coverage goals (e.g., 80% code coverage)
     - 🧩 Test categories (unit, integration, etc.)
     - 📦 Required testing frameworks and libraries
     - 🔍 Priority areas for testing
     - ⚙️ Mock strategy for external dependencies

**4.3 Create Test Projects**
   - Copilot will typically create test projects such as:
     - `ContosoUniversity.Tests` - Unit tests
     - `ContosoUniversity.IntegrationTests` - Integration tests (optional)

**4.4 Install Testing Dependencies**
   - Copilot will handle this for you automatically

**4.5 Generate Test Classes**
   - Copilot will generate tests for:
     - **Controllers**: HTTP request/response handling, routing, model validation
     - **Services**: Business logic, data transformations
     - **Data Access**: Database operations, queries, migrations
     - **Models**: Validation rules, computed properties
   
**4.6 Review Generated Tests**
   - Examine test structure and coverage, look for classes such as this example for `StudentsController`:
     ```csharp
     public class StudentsControllerTests
     {
         [Fact]
         public async Task Index_ReturnsViewWithStudentList()
         {
             // Arrange
             var mockRepo = new Mock<IStudentRepository>();
             mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(GetTestStudents());
             var controller = new StudentsController(mockRepo.Object);
             
             // Act
             var result = await controller.Index();
             
             // Assert
             var viewResult = Assert.IsType<ViewResult>(result);
             var model = Assert.IsAssignableFrom<IEnumerable<Student>>(viewResult.Model);
             Assert.Equal(3, model.Count());
         }
         
         // Additional test methods...
     }
     ```

**4.7 Run Initial Test Suite**
   - Execute tests from Visual Studio Test Explorer (Test → Test Explorer)
   - Or via command line:
     ```bash
     dotnet test --logger "console;verbosity=detailed"
     ```
   
   - Review the output:
     - ✅ Passed tests (green)
     - ❌ Failed tests (red)
     - ⚠️ Skipped tests (yellow)

**4.8 Analyze Test Results**
   - Copilot will generate a `test-implementation-summary.md` with:
     - Total tests created
     - Pass/fail statistics
     - Code coverage percentage by project
     - Areas needing additional coverage
     - Known issues or test failures
   
   - Review code coverage:
     ```bash
     dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
     ```

**4.9 Address Test Failures**
   - **For each failing test**:
     1. Read the error message carefully
     2. Determine if it's a test issue or application bug
     3. Copy the test failure details to Copilot:
        ```
        This test is failing:
        [paste test name and full error message]
        
        Please help me diagnose whether this is a test implementation issue or an actual bug in the application code.
        ```
     4. Apply the recommended fix
     5. Re-run the test

   - **Common failure categories**:
     - Mock configuration issues
     - Async/await misuse
     - Database context disposal
     - Null reference errors
     - Dependency injection setup

**4.10 Handle Acceptable Failures (Technical Debt)**
   - Some tests may fail due to:
     - Complex external dependencies (third-party APIs)
     - Windows-specific features (MSMQ) not yet migrated
     - Integration points not ready for testing
   
   - Document these with `[Fact(Skip = "reason")]`:
     ```csharp
     [Fact(Skip = "MSMQ functionality will be replaced with Azure Service Bus in Stage 5")]
     public void NotificationService_SendMessage_UsesMessageQueue()
     {
         // Test implementation...
     }
     ```

**4.11 Improve Coverage in Critical Areas**
   - Identify untested or under-tested areas:
     ```
     My coverage report shows only 45% coverage in the Services folder. Generate additional tests to improve coverage of critical business logic.
     ```
   - Focus on:
     - Complex business rules
     - Error handling paths
     - Edge cases and boundary conditions
     - Security-sensitive operations

**4.12 Establish Testing Standards**
   - Document your team's testing guidelines:
     - Naming conventions (e.g., `MethodName_Scenario_ExpectedBehavior`)
     - Arrange-Act-Assert pattern
     - Mock vs. integration testing decisions
     - Code coverage targets
   
   - Add these to a `TESTING.md` file in your repository

**4.13 Commit Your Test Suite**
   ```bash
   git checkout -b upgrade-to-NET9-unit-test
   git add .
   git commit -m "Add comprehensive unit test suite with 75% coverage"
   git push origin upgrade-to-NET9-unit-test
   ```

> **💡 Pro Tip**: Don't aim for 100% coverage. Focus on testing critical business logic and complex algorithms. Simple property getters/setters often don't provide enough value to justify the test maintenance burden.

> **🎯 Coverage Targets**: Industry standards suggest 70-80% code coverage for business applications. Higher coverage is better, but diminishing returns set in above 85%. Focus on meaningful tests over arbitrary metrics.

> **⚠️ Note**: It's acceptable to defer fixing some tests if they depend on infrastructure not yet migrated (like MSMQ → Service Bus). Document these as known items and address them in later stages.

**Expected Outcomes**:
- ✅ Comprehensive test project structure created
- ✅ 70-80% code coverage achieved (or defined target)
- ✅ Critical business logic fully tested
- ✅ All or most tests passing
- ✅ Test failures documented with remediation plans
- ✅ Testing framework integrated into CI/CD
- ✅ Baseline established for future development

---

### 🐳 Stage 5: Containerization

**Objective**: Package the application into an optimized Docker container, enabling consistent deployment across environments and preparing for cloud-native hosting.

**Why This Matters**: Containerization eliminates "works on my machine" problems, simplifies deployment, enables horizontal scaling, and is a prerequisite for modern cloud platforms like Azure Container Apps, Kubernetes, and AWS ECS.

#### Step-by-Step Instructions

**5.1 Verify Prerequisites**
   - Ensure Docker Desktop is installed and running on your machine
   - Verify Docker is accessible:
     ```bash
     docker --version
     docker ps
     ```
   - Confirm your application runs correctly on .NET 9 before containerizing

**5.2 Request Dockerfile Generation**
   - In the GitHub Copilot chat window, provide a detailed request:
     ```
     Help me containerize this .NET 9 application to prepare it for deployment in a cloud-native environment. Please:
     
     1. Create an optimized multi-stage Dockerfile following best practices
     2. Target deployment to Azure Container Apps using AZD (Azure Developer CLI)
     3. Optimize the image size using appropriate base images
     4. Include health checks for container orchestration
     5. Configure for both development and production environments
     6. Ensure all dependencies (database, configuration) are properly handled
     ```

**5.3 Review the Generated Dockerfile**
   - Copilot will create a `Dockerfile` in your solution root
   - **Understand the multi-stage build**:
   
   ```dockerfile
   # Stage 1: Build
   FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
   WORKDIR /src
   
   # Copy and restore dependencies (layer caching optimization)
   COPY ["ContosoUniversity/ContosoUniversity.csproj", "ContosoUniversity/"]
   RUN dotnet restore "ContosoUniversity/ContosoUniversity.csproj"
   
   # Copy source and build
   COPY . .
   WORKDIR "/src/ContosoUniversity"
   RUN dotnet build "ContosoUniversity.csproj" -c Release -o /app/build
   
   # Stage 2: Publish
   FROM build AS publish
   RUN dotnet publish "ContosoUniversity.csproj" -c Release -o /app/publish /p:UseAppHost=false
   
   # Stage 3: Runtime
   FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
   WORKDIR /app
   EXPOSE 8080
   EXPOSE 8081
   
   COPY --from=publish /app/publish .
   ENTRYPOINT ["dotnet", "ContosoUniversity.dll"]
   ```
   
   - **Key benefits of multi-stage build**:
     - ✅ Final image only contains runtime dependencies (smaller size)
     - ✅ SDK tools not included in production image (more secure)
     - ✅ Layer caching speeds up subsequent builds

**5.4 Create .dockerignore File**
   - Copilot should also generate a `.dockerignore` file:
   
   ```
   **/.vs
   **/.vscode
   **/bin
   **/obj
   **/.git
   **/node_modules
   **/*.user
   **/TestResults
   **/.vs/
   **/packages/
   ```
   
   - This prevents unnecessary files from being copied into the container, reducing build time and image size

**5.5 Build the Docker Image**
   - Build your container locally:
     ```bash
     # Navigate to solution root
     cd c:\code\gbb\app-mod-dotnet
     
     # Build with a versioned tag
     docker build -t contoso-university:latest -t contoso-university:1.0.0 .
     ```
   
   - Monitor the build output for:
     - ✅ All stages complete successfully
     - ✅ No warnings about missing files
     - ✅ Final image size reported
   
   - Expected build time: 2-5 minutes on first build (faster on subsequent builds due to layer caching)

**5.6 Verify Image Size**
   - Check the image size:
     ```bash
     docker images contoso-university
     ```
   
   - **Good targets**:
     - **Excellent**: < 250 MB (minimal dependencies)
     - **Good**: 250-500 MB (typical .NET web app)
     - **Acceptable**: 500 MB - 1 GB (complex dependencies)
     - **Needs optimization**: > 1 GB
   
   - If too large, ask Copilot:
     ```
     My Docker image is [size]MB. Please suggest optimizations to reduce the image size.
     ```

**5.7 Create Local Testing Configuration**
   - Copilot may generate a `docker-compose.yml` for local development:
   
   ```yaml
   version: '3.8'
   
   services:
     web:
       build:
         context: .
         dockerfile: Dockerfile
       ports:
         - "8080:8080"
       environment:
         - ASPNETCORE_ENVIRONMENT=Development
         - ConnectionStrings__DefaultConnection=Server=sql;Database=ContosoUniversity;User=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true
       depends_on:
         - sql
       networks:
         - contoso-network
     
     sql:
       image: mcr.microsoft.com/mssql/server:2022-latest
       environment:
         - ACCEPT_EULA=Y
         - SA_PASSWORD=YourStrong@Passw0rd
       ports:
         - "1433:1433"
       volumes:
         - sql-data:/var/opt/mssql
       networks:
         - contoso-network
   
   volumes:
     sql-data:
   
   networks:
     contoso-network:
       driver: bridge
   ```
   
   - This allows you to test the application with a containerized database

**5.8 Run the Container Locally**
   - **Option A: Run standalone container**:
     ```bash
     docker run -d -p 8080:8080 --name contoso-app contoso-university:latest
     ```
   
   - **Option B: Run with docker-compose** (recommended for testing):
     ```bash
     docker-compose up -d
     ```
   
   - **Verify container is running**:
     ```bash
     docker ps
     ```

**5.9 Test the Containerized Application**
   - Open your browser to `http://localhost:8080`
   - Verify all functionality works:
     - ✅ Home page loads
     - ✅ Database connection succeeds
     - ✅ Student management works
     - ✅ Course and instructor pages function
     - ✅ Static assets (CSS, JS) load correctly
   
   - **Check container logs if issues occur**:
     ```bash
     docker logs contoso-app
     # Or with docker-compose:
     docker-compose logs web
     ```

**5.10 Troubleshoot Common Issues**
   - **Connection string issues**: Ensure database host matches service name in docker-compose
   - **Port conflicts**: Change host port if 8080 is occupied: `-p 8081:8080`
   - **Static files not serving**: Verify `UseStaticFiles()` is in Program.cs
   - **Database not seeding**: Check connection string and database initialization logic
   
   - For any issues, ask Copilot:
     ```
     My containerized application shows this error:
     [paste error from logs]
     
     Please help me diagnose and fix this containerization issue.
     ```

**5.11 Prepare for Azure Deployment**
   - Copilot should generate Azure-specific configuration files:
     - `azure.yaml` - Azure Developer CLI configuration
     - `.azure/` folder - Azure resource definitions
     - Bicep files or ARM templates for infrastructure
   
   - Review these files to understand what resources will be provisioned

**5.12 Tag and Document Your Image**
   - Tag the working image:
     ```bash
     docker tag contoso-university:latest contoso-university:stable
     docker tag contoso-university:latest contoso-university:v1.0.0
     ```
   
   - Document the container in README:
     - Port mappings
     - Required environment variables
     - Volume mounts (if any)
     - Dependencies

**5.13 Clean Up Test Containers**
   ```bash
   # Stop and remove containers
   docker-compose down
   # Or for standalone:
   docker stop contoso-app
   docker rm contoso-app
   ```

**5.14 Commit Containerization Files**
   ```bash
   git checkout -b upgrade-to-NET9-containerize
   git add Dockerfile .dockerignore docker-compose.yml azure.yaml .azure/
   git commit -m "Add Docker containerization with Azure deployment config"
   git push origin upgrade-to-NET9-containerize
   ```

> **💡 Pro Tip**: Use `docker build --progress=plain .` to see detailed output during builds. This is invaluable for troubleshooting build failures.

> **🎯 Health Checks**: Consider adding health check endpoints to your application (`/health`, `/ready`) that container orchestrators can use to determine if your container is healthy and ready to receive traffic.

> **⚠️ Security**: Never hardcode secrets (connection strings, API keys, passwords) in your Dockerfile. Use environment variables or Azure Key Vault for production deployments.

> **📊 Image Optimization Tips**:
> - Use `dotnet publish -c Release` (done in multi-stage builds)
> - Trim unused assemblies with `<PublishTrimmed>true</PublishTrimmed>`
> - Use `alpine` variants of base images when possible
> - Remove development tools and symbols from final image
> - Minimize layers by combining RUN commands where appropriate

**Expected Outcomes**:
- ✅ Optimized multi-stage Dockerfile created
- ✅ .dockerignore file prevents bloat
- ✅ Docker image builds successfully
- ✅ Image size is reasonable (< 500 MB preferred)
- ✅ Application runs correctly in container
- ✅ docker-compose.yml enables local testing
- ✅ Azure deployment configuration generated
- ✅ Container health and logging verified
- ✅ Ready for cloud deployment

---

## 🎯 Next Steps After Modernization

Once you've completed all stages, your application is ready for modern cloud deployment! Consider these follow-up activities:

### Deployment Options
- **Azure Container Apps**: Fully managed container hosting with auto-scaling
- **Azure Kubernetes Service (AKS)**: Full Kubernetes orchestration for complex scenarios
- **Azure App Service**: Traditional web hosting with container support
- **Azure Container Instances**: Simple, on-demand container execution

### Continuous Improvement
- Set up **CI/CD pipelines** (GitHub Actions, Azure DevOps)
- Implement **monitoring and observability** (Application Insights, Azure Monitor)
- Configure **auto-scaling** policies
- Add **feature flags** for gradual rollouts
- Implement **blue-green or canary deployments**

### Cloud-Native Enhancements
- Migrate remaining Windows dependencies to Azure services
- Implement **Azure Service Bus** (replacing MSMQ)
- Use **Azure Blob Storage** (replacing file system)
- Add **Azure Key Vault** for secrets management
- Configure **Azure SQL Database** with geo-replication

### Share Your Success
- Document lessons learned
- Create training materials for your team
- Share your modernization story with the community
- Contribute improvements back to this sample repository

---

<div align="center">

# 🎉 CONGRATULATIONS! 🎉

</div>

---

<div align="center">

### 🏆 **You've Successfully Completed the .NET Modernization Journey!** 🏆

</div>

**You have achieved:**

<table>
<tr>
<td align="center" width="25%">
<img src="https://img.shields.io/badge/Framework-✅_Upgraded-success?style=for-the-badge" alt="Framework Upgraded"/>
<br><b>Framework Upgrade</b><br>
.NET Framework 4.8 → .NET 9
</td>
<td align="center" width="25%">
<img src="https://img.shields.io/badge/Security-✅_Hardened-success?style=for-the-badge" alt="Security Hardened"/>
<br><b>Security Enhanced</b><br>
All CVEs resolved & best practices implemented
</td>
<td align="center" width="25%">
<img src="https://img.shields.io/badge/Testing-✅_Covered-success?style=for-the-badge" alt="Testing Covered"/>
<br><b>Quality Assured</b><br>
Comprehensive test suite & coverage
</td>
<td align="center" width="25%">
<img src="https://img.shields.io/badge/Container-✅_Ready-success?style=for-the-badge" alt="Container Ready"/>
<br><b>Cloud Ready</b><br>
Containerized & deployment-ready
</td>
</tr>
</table>

<br>

<div align="center">

### 🌟 **This is a Significant Achievement!** 🌟

You've transformed legacy code into a **modern, secure, tested, and cloud-native application**.<br>
You're now positioned for success in the **modern cloud-native world**!

</div>

<div align="center">

**🚀 Your application is ready for:**
- ☁️ Cloud deployment (Azure, AWS, GCP)
- 📈 Horizontal scaling and high availability
- 🔄 CI/CD automation
- 🛡️ Enterprise security standards
- 🌍 Cross-platform operation

</div>

<br>

> **💪 What's Next?** Deploy with confidence, implement monitoring, and continue to iterate and improve. The hard work of modernization is behind you—now reap the benefits of modern .NET!

---

## 🤝 Contributing

Contributions are welcome! This repository is meant to be a learning resource for the community.

### How to Contribute

1. **Fork** the repository
2. **Create a feature branch** (`git checkout -b feature/improvement`)
3. **Make your changes** and commit (`git commit -am 'Add new modernization example'`)
4. **Push** to your branch (`git push origin feature/improvement`)
5. **Open a Pull Request** with a clear description

### Areas for Contribution

- 📖 Additional documentation
- 🐛 Bug fixes
- ✨ New modernization scenarios
- 🎨 UI improvements
- 🧪 More test coverage
- 🌐 Internationalization examples

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 📞 Support & Resources

- 🐞 [Report Issues](https://github.com/Azure-Samples/dotnet-migration-copilot-samples/issues)
- 💬 [Discussions](https://github.com/Azure-Samples/dotnet-migration-copilot-samples/discussions)
- 📖 [.NET Upgrade Assistant](https://dotnet.microsoft.com/platform/upgrade-assistant)
- 🤖 [GitHub Copilot Documentation](https://docs.github.com/copilot)
- ☁️ [Azure Documentation](https://docs.microsoft.com/azure)

---

## 🎓 Learning Resources

- [.NET Framework to .NET 8 Migration Guide](https://learn.microsoft.com/dotnet/core/porting/)
- [ASP.NET to ASP.NET Core Migration](https://learn.microsoft.com/aspnet/core/migration/proper-to-2x/)
- [Azure Container Apps Documentation](https://learn.microsoft.com/azure/container-apps/)
- [GitHub Copilot for Azure](https://docs.github.com/copilot/github-copilot-chat/copilot-chat-in-ides/using-github-copilot-chat-in-your-ide)

---

**Made with ❤️ by the Migrate and Modernize GBB Team**

*Happy Modernizing! 🚀*
