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
| `assess` | **Assessment** | Analysis results, compatibility reports, and modernization recommendations |
| `upgrade` | **Framework Upgrade** | Upgrade to .NET 8, project file modernization (SDK-style) |
| `migrate` | **Code Migration** | ASP.NET MVC to ASP.NET Core MVC migration |
| `cve-check` | **Security Scan** | Vulnerability assessment and package updates |
| `build-fix` | **Build Resolution** | Fix compilation errors and warnings |
| `unit-test` | **Testing** | Unit test implementation and test coverage |
| `containerize` | **Containerization** | Dockerfile, container optimization, multi-stage builds |
| `deploy` | **Azure Deployment** | Azure Container Apps deployment, infrastructure as code |

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

Follow this step-by-step guide to modernize the Contoso University application using GitHub Copilot App Modernization Plugin.

### Stage 1: Assessment

**Objective**: Analyze the current application and create a modernization plan.

1. **Open the project** in VS Code or Visual Studio 2022
2. ...

---

### Stage 2: Upgrade Framework

**Objective**: Upgrade from .NET Framework 4.8 to .NET 8.

1. ...

---

### Stage 3: Migrate to ASP.NET Core

**Objective**: Migrate from ASP.NET MVC 5 to ASP.NET Core MVC.

1. ...

---

### Stage 4: CVE Check & Security

**Objective**: Identify and fix security vulnerabilities.

1. ...
---

### Stage 5: Build Fixes

**Objective**: Resolve all compilation errors and warnings.

1. ...

---

### Stage 6: Unit Testing

**Objective**: Add tests to ensure functionality during modernization.

1. ....

---

### Stage 7: Containerization

**Objective**: Create Docker containers for the application.

1. ....
---

### Stage 8: Azure Deployment

**Objective**: Deploy to Azure Container Apps with supporting services.

1. ....

**Reference**: Check the `deploy` branch for complete deployment scripts and configuration.

---

## 📸 Screenshots

### Legacy Application (main branch)

**Home Page**
> TODO: Add screenshot of the home page showing enrollment statistics

**Student List**
> TODO: Add screenshot of student management page with pagination

**Course Management**
> TODO: Add screenshot of course creation/editing

**Instructor Dashboard**
> TODO: Add screenshot showing instructor courses and students

### Modernized Application (deploy branch)

**Running in Container**
> TODO: Add screenshot of `docker ps` showing running container

**Azure Container Apps**
> TODO: Add screenshot from Azure Portal showing Container App

**Application Insights Dashboard**
> TODO: Add screenshot of monitoring dashboard

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
