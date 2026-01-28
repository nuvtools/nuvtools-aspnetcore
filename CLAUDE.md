# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

NuvTools.AspNetCore is a suite of helper libraries for ASP.NET Core applications targeting .NET 8, .NET 9, and .NET 10. The solution contains:

- **NuvTools.AspNetCore**: General-purpose library with composite localization
- **NuvTools.AspNetCore.EntityFrameworkCore**: Entity Framework Core helpers with database migration extensions
- **NuvTools.AspNetCore.Blazor**: Blazor JavaScript interop services (clipboard, local/session storage)
- **NuvTools.AspNetCore.Blazor.MudBlazor**: MudBlazor components, converters, and utilities
- **NuvTools.AspNetCore.Tests**: Test project using NUnit

All libraries are published as NuGet packages.

## Build and Test Commands

**Note:** This solution uses the modern `.slnx` (XML-based solution) format introduced in Visual Studio 2022 v17.11.

### Build the solution
```bash
dotnet build NuvTools.AspNetCore.slnx
```

### Build for specific configuration
```bash
dotnet build NuvTools.AspNetCore.slnx --configuration Release
```

### Run all tests
```bash
dotnet test NuvTools.AspNetCore.slnx
```

### Run tests for specific project
```bash
dotnet test tests/NuvTools.AspNetCore.Tests/NuvTools.AspNetCore.Tests.csproj
```

### Build NuGet packages
All projects have `GeneratePackageOnBuild` set to `true`, so packages are automatically generated during Release builds:
```bash
dotnet build NuvTools.AspNetCore.slnx --configuration Release
```

### Clean build artifacts
```bash
dotnet clean NuvTools.AspNetCore.slnx
```

## Architecture and Key Components

### Multi-Targeting Strategy
All library projects target three frameworks: `net8`, `net9`, and `net10.0`. Some dependencies (like `Microsoft.JSInterop`) are conditionally referenced based on the target framework to use framework-specific versions.

### NuvTools.AspNetCore Library

This library provides foundational helpers organized into namespaces:

#### Localization (`NuvTools.AspNetCore.Localization`)
- **CompositeLocalizer**: Enables multiple localization sources with fallback mechanism. Supports prefixed keys (e.g., `"Errors:ValidationFailed"`) to target specific localizers.
- **LocalizationServiceExtensions**: Extension method `AddCompositeLocalizer()` for registering the composite localization system with named and unnamed resource types.

Key pattern: The CompositeLocalizer allows you to combine multiple IStringLocalizer instances and resolve localization keys with prefix-based routing or fallback logic.

### NuvTools.AspNetCore.Blazor Library

Provides JavaScript interop services for Blazor applications:

- **IClipboardService / ClipboardService**: Clipboard read/write operations
- **ILocalStorageService / LocalStorageService**: Browser localStorage access with JSON serialization
- **ISessionStorageService / SessionStorageService**: Browser sessionStorage access with JSON serialization

Register services using `AddBlazorServices()` extension method.

### NuvTools.AspNetCore.Blazor.MudBlazor Library

MudBlazor-specific components and utilities:

#### Services
- **ILoadingService / LoadingService**: Counter-based loading indicator with nested call support. Use `RunAsync()` for exception-safe loading.

#### Components
- **MudTablePageBase<TItem, TFilter, TOrdering>**: Abstract base class for server-side paged MudTable with session storage filter persistence. Override `OnFetchDataFailed()` to customize error handling.

#### Converters (`NuvTools.AspNetCore.Blazor.MudBlazor.Converters`)
- **PatternStringConverter**: Pattern-based input masking for MudTextField. Pattern chars: `A`=alphanumeric, `N`=numeric, `L`=letter.
- **UpperCaseConverter**: Converts input to uppercase.
- **VinConverter**: Vehicle Identification Number formatting.

#### Country-Specific Converters
- **Brazil** (`Brazil.Converters.BrazilianDocumentConverters`): MobilePhone, LandlinePhone, Cpf, Cnpj, Cep
- **United States** (`UnitedStates.Converters.USDocumentConverters`): Phone, MobilePhone, LandlinePhone, Ssn, ZipCode, ZipCodePlus4
- **Mercosul** (`Mercosul.Converters.MercosulDocumentConverters`): License plate formats

#### Validators
- **MudFormValidatorBase**: Base class for MudBlazor form validation integration.

### NuvTools.AspNetCore.EntityFrameworkCore Library

Provides Entity Framework Core helpers:

- **ApplicationBuilderExtensions**: Contains `DatabaseMigrate<TContext>()` extension method for running EF Core migrations during application startup with optional timeout configuration.

## Code Style and Conventions

- **Nullable reference types** are enabled (`<Nullable>enable</Nullable>`)
- **Implicit usings** are enabled (`<ImplicitUsings>enable</ImplicitUsings>`)
- **Code style enforcement** is enabled during build (`<EnforceCodeStyleInBuild>True</EnforceCodeStyleInBuild>`)
- **XML documentation generation** is required (`<GenerateDocumentationFile>True</GenerateDocumentationFile>`)
- Check `.editorconfig` for additional style rules

## Dependencies

### NuvTools.AspNetCore
- Microsoft.Extensions.Localization.Abstractions
- Microsoft.Extensions.Logging.Abstractions

### NuvTools.AspNetCore.EntityFrameworkCore
- NuvTools.Data.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Relational
- Microsoft.AspNetCore.App (framework reference)

### NuvTools.AspNetCore.Blazor
- Microsoft.JSInterop (version varies by target framework)

### NuvTools.AspNetCore.Blazor.MudBlazor
- MudBlazor
- NuvTools.AspNetCore.Blazor (project reference)

## Testing

Tests use NUnit framework (version 4.4.0) with NUnit3TestAdapter. The test project targets only `net10.0`.

### Run a single test
```bash
dotnet test --filter "FullyQualifiedName~TestClassName.TestMethodName"
```
