# .NET 9.0 Upgrade Report

## Project target framework modifications

| Project name                | Old Target Framework | New Target Framework | Commits                                      |
|:----------------------------|:--------------------:|:--------------------:|----------------------------------------------|
| ContosoUniversity.csproj    | net48                | net9.0               | 2d94c49b, f72ac826, 60fa10af                 |

## NuGet Packages

| Package Name                                      | Old Version | New Version | Commit Id                                    |
|:--------------------------------------------------|:-----------:|:-----------:|----------------------------------------------|
| Antlr                                             | 3.4.1.9004  | Removed     | 2d94c49b                                     |
| Antlr4                                            | -           | 4.6.6       | 2d94c49b                                     |
| Microsoft.AspNet.Mvc                              | 5.2.9       | Removed     | 2d94c49b                                     |
| Microsoft.AspNet.Razor                            | 3.2.9       | Removed     | 2d94c49b                                     |
| Microsoft.AspNet.Web.Optimization                 | 1.1.3       | Removed     | 2d94c49b                                     |
| Microsoft.AspNet.WebPages                         | 3.2.9       | Removed     | 2d94c49b                                     |
| Microsoft.Bcl.AsyncInterfaces                     | 1.1.1       | 9.0.13      | 2d94c49b                                     |
| Microsoft.Bcl.HashCode                            | 1.1.1       | 6.0.0       | 2d94c49b                                     |
| Microsoft.CodeDom.Providers.DotNetCompilerPlatform| 2.0.1       | Removed     | 2d94c49b                                     |
| Microsoft.Data.SqlClient                          | 2.1.4       | 6.1.4       | f72ac826                                     |
| Microsoft.Data.SqlClient.SNI.runtime              | 2.1.1       | 6.0.2       | 99366a30                                     |
| Microsoft.EntityFrameworkCore                     | 3.1.32      | 9.0.13      | f72ac826                                     |
| Microsoft.EntityFrameworkCore.Abstractions        | 3.1.32      | 9.0.13      | f72ac826                                     |
| Microsoft.EntityFrameworkCore.Analyzers           | 3.1.32      | 9.0.13      | f72ac826                                     |
| Microsoft.EntityFrameworkCore.Relational          | 3.1.32      | 9.0.13      | f72ac826                                     |
| Microsoft.EntityFrameworkCore.SqlServer           | 3.1.32      | 9.0.13      | f72ac826                                     |
| Microsoft.EntityFrameworkCore.Tools               | 3.1.32      | 9.0.13      | f72ac826                                     |
| Microsoft.Extensions.Caching.Abstractions         | 3.1.32      | 9.0.13      | f72ac826                                     |
| Microsoft.Extensions.Caching.Memory               | 3.1.32      | 9.0.13      | f72ac826                                     |
| Microsoft.Extensions.Configuration                | 3.1.32      | 9.0.13      | f72ac826                                     |
| Microsoft.Extensions.Configuration.Abstractions   | 3.1.32      | 9.0.13      | f72ac826                                     |
| Microsoft.Extensions.Configuration.Binder         | 3.1.32      | 9.0.13      | f72ac826                                     |
| Microsoft.Extensions.DependencyInjection          | 3.1.32      | 9.0.13      | f72ac826                                     |
| Microsoft.Extensions.DependencyInjection.Abstractions | 3.1.32  | 9.0.13      | f72ac826                                     |
| Microsoft.Extensions.Logging                      | 3.1.32      | 9.0.13      | f72ac826                                     |
| Microsoft.Extensions.Logging.Abstractions         | 3.1.32      | 9.0.13      | f72ac826                                     |
| Microsoft.Extensions.Options                      | 3.1.32      | 9.0.13      | f72ac826                                     |
| Microsoft.Extensions.Primitives                   | 3.1.32      | 9.0.13      | f72ac826                                     |
| Microsoft.Identity.Client                         | 4.21.1      | 4.82.1      | f72ac826                                     |
| Microsoft.Web.Infrastructure                      | 2.0.1       | Removed     | 2d94c49b                                     |
| MSMQ.Messaging                                    | -           | 1.0.4       | 315f9d6b                                     |
| NETStandard.Library                               | 2.0.3       | Removed     | 2d94c49b                                     |
| Newtonsoft.Json                                   | 13.0.3      | 13.0.4      | f72ac826                                     |
| System.Buffers                                    | 4.5.1       | Removed     | 2d94c49b                                     |
| System.Collections.Immutable                      | 1.7.1       | 9.0.13      | 2d94c49b                                     |
| System.ComponentModel.Annotations                 | 4.7.0       | Removed     | 2d94c49b                                     |
| System.Configuration.ConfigurationManager         | -           | 10.0.3      | f72ac826                                     |
| System.Diagnostics.DiagnosticSource               | 4.7.1       | 9.0.13      | 2d94c49b                                     |
| System.Memory                                     | 4.5.4       | Removed     | 2d94c49b                                     |
| System.Numerics.Vectors                           | 4.5.0       | Removed     | 2d94c49b                                     |
| System.Runtime.CompilerServices.Unsafe            | 4.5.3       | 6.1.2       | 2d94c49b                                     |
| System.Threading.Tasks.Extensions                 | 4.5.4       | Removed     | 2d94c49b                                     |
| WebGrease                                         | 1.5.2       | Removed     | 2d94c49b                                     |

## All commits

| Commit ID | Description                                                                                                   |
|:----------|:--------------------------------------------------------------------------------------------------------------|
| 60fa10af  | Store final changes for step 'Convert ContosoUniversity.csproj to SDK-style project'                         |
| 15b9d44d  | Replaced HttpStatusCodeResult with BadRequest() in InstructorsController.cs                                   |
| 577ecbfb  | Replaced System.Web.Mvc with Microsoft.AspNetCore.Mvc in InstructorsController.cs                            |
| bb0a474a  | RouteCollection feature upgrade completed: added route mapping to Program.cs, removed RouteConfig.cs          |
| cf6d4d64  | System.Web.Optimization bundling upgrade completed: replaced bundle renders with direct tags                  |
| 8c8d8cde  | Fixed Bind attribute usage in DepartmentsController.cs                                                        |
| 4844be39  | Fixed Bind attribute usage in CoursesController.cs                                                            |
| cc881cbc  | Replaced HttpNotFound() with NotFound() in InstructorsController.cs                                           |
| 1cfad096  | Replaced HttpNotFound() with NotFound() in CoursesController.cs                                               |
| eac32387  | Replaced HttpNotFound() with NotFound() in CoursesController.cs                                               |
| 5051f0f6  | Replaced HttpStatusCodeResult with StatusCode in InstructorsController.cs                                     |
| fd8bfb8c  | Replaced HttpStatusCodeResult with StatusCode in InstructorsController.cs                                     |
| 65d478fe  | Replaced HttpStatusCodeResult with StatusCode in InstructorsController.cs                                     |
| c007318e  | Replaced HttpStatusCodeResult with StatusCode in CoursesController.cs                                         |
| d9da9c15  | Replaced HttpStatusCodeResult with BadRequest() in CoursesController.cs                                       |
| 38304585  | Added using directive for SelectList in DepartmentsController.cs                                              |
| 4f1d8c1a  | Replaced System.Web.Mvc with Microsoft.AspNetCore.Mvc in StudentsController.cs                               |
| 90c2b398  | Replaced System.Web.Mvc with Microsoft.AspNetCore.Mvc in HomeController.cs                                   |
| 2d94c49b  | Migrate to SDK-style project and .NET 9; remove packages.config                                               |
| d469cf9e  | Global.asax.cs conversion completed: moved initialization to Program.cs, removed Global.asax files            |
| de00368c  | System.Messaging to MSMQ upgrade refined: updated dependency injection in controllers                         |
| 16c351d9  | Replaced HttpStatusCodeResult with StatusCodeResult in DepartmentsController.cs                               |
| fef20f69  | Replaced HttpStatusCodeResult with StatusCodeResult in DepartmentsController.cs                               |
| 99366a30  | Updated SqlClient.SNI.runtime version to 6.0.2                                                                |
| f16f91ff  | Replaced Server.MapPath with IWebHostEnvironment.WebRootPath in CoursesController.cs                         |
| 055aba25  | Fixed Bind attribute usage in DepartmentsController.cs                                                        |
| 7865f8d3  | Fixed Bind attribute usage in StudentsController.cs                                                           |
| c2ff0d5b  | Replaced HttpNotFound() with NotFound() in DepartmentsController.cs                                           |
| 27f9ad11  | Replaced System.Web with Microsoft.AspNetCore.Mvc.Rendering in CoursesController.cs                          |
| aef3c97c  | Replaced System.Web.Mvc with Microsoft.AspNetCore.Mvc in DepartmentsController.cs                            |
| 315f9d6b  | System.Messaging to MSMQ upgrade completed: updated to MSMQ.Messaging, added DI registration                 |
| ce9656e3  | Fixed Bind attribute usage in StudentsController.cs                                                           |
| f614c0b7  | Fixed Bind attribute usage in InstructorsController.cs                                                        |
| bf13accf  | Replaced System.Web.Mvc with Microsoft.AspNetCore.Mvc in NotificationsController.cs                          |
| 516655f3  | Replaced HttpNotFound() with NotFound() in StudentsController.cs                                              |
| 2efa5664  | Replaced HttpNotFound() with NotFound() in CoursesController.cs                                               |
| a4da4227  | Replaced HttpStatusCodeResult with BadRequest() in StudentsController.cs                                      |
| b3989367  | Replaced HttpStatusCodeResult with StatusCodeResult in StudentsController.cs                                  |
| f6ec78e0  | Replaced HttpStatusCodeResult with StatusCodeResult in StudentsController.cs                                  |

## Project feature upgrades

### ContosoUniversity.csproj

Here is what changed for the project during upgrade:

- **Project conversion**: Converted from .NET Framework 4.8 to SDK-style project targeting .NET 9.0
- **System.Web.Optimization bundling and minification**: Replaced all @Scripts.Render and @Styles.Render calls with direct script and link tags in all Razor views (_Layout.cshtml and form views), removed BundleConfig.cs and related references
- **RouteCollection feature**: Converted route registration from RouteConfig.cs to ASP.NET Core routing in Program.cs using MapControllerRoute, removed RouteConfig.cs
- **System.Messaging to MSMQ**: Updated NotificationService to use MSMQ.Messaging package (version 1.0.4) instead of System.Messaging, implemented dependency injection for NotificationService and IConfiguration
- **Global.asax.cs conversion**: Migrated application initialization from Global.asax.cs to Program.cs with proper ASP.NET Core setup including middleware pipeline, routing, and database initialization; removed Global.asax, Global.asax.cs, and FilterConfig.cs
- **Controller API updates**: Replaced obsolete System.Web.Mvc types with Microsoft.AspNetCore.Mvc equivalents:
  - HttpStatusCodeResult → StatusCodeResult or BadRequest()
  - HttpNotFound() → NotFound()
  - HttpPostedFileBase → IFormFile
  - Server.MapPath → IWebHostEnvironment.WebRootPath
  - TryUpdateModel → TryUpdateModelAsync
  - Bind attribute syntax updated to ASP.NET Core format
- **Configuration**: Created appsettings.json with connection strings and notification queue settings, replacing Web.config-based configuration
- **Dependency injection**: Implemented constructor injection for SchoolContext and NotificationService in all controllers inheriting from BaseController
- **Assembly info**: Disabled auto-generation of AssemblyInfo to avoid conflicts with existing Properties\AssemblyInfo.cs

## Next steps

- Test the application thoroughly to ensure all functionality works correctly
- Consider removing Web.config and related files if they're no longer needed
- Review and potentially migrate any remaining configuration from Web.config to appsettings.json
- Consider implementing proper logging using ILogger instead of Debug.WriteLine
- Update views to use tag helpers instead of HTML helpers for a more modern ASP.NET Core approach
