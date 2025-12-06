# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

NuvTools.AspNetCore is a suite of helper libraries for ASP.NET Core applications targeting .NET 8, .NET 9, and .NET 10. The solution contains:

- **NuvTools.AspNetCore**: General-purpose library with common helpers for ASP.NET Core applications
- **NuvTools.AspNetCore.EntityFrameworkCore**: Entity Framework Core helpers and extensions
- **NuvTools.AspNetCore.Tests**: Test project using NUnit

Both libraries are published as NuGet packages with strong-name signing enabled.

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
Both projects have `GeneratePackageOnBuild` set to `true`, so packages are automatically generated during Release builds:
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

#### Mapper (`NuvTools.AspNetCore.Mapper`)
- **ServiceWithMapperBase<TDTO, TEntity>**: Abstract base class providing AutoMapper integration for service classes. Includes helper methods for converting between DTOs and entities in various collection types (IEnumerable, IList, arrays).

#### JSInterop (`NuvTools.AspNetCore.JSInterop`)
- **ClipboardService**: Provides JavaScript interop for clipboard operations (`ReadTextAsync`, `WriteTextAsync`).

### NuvTools.AspNetCore.EntityFrameworkCore Library

This library extends the base library with Entity Framework Core functionality:

#### Mapper (`NuvTools.AspNetCore.EntityFrameworkCore.Mapper`)
- **ServiceWithCrudBase<TContext, TDTO, TEntity, TKey>**: Abstract base class inheriting from `ServiceWithMapperBase` that adds CRUD operations with AutoMapper integration. Provides:
  - `FindAsync()`: Retrieve entities by ID or composite keys
  - `FindFromExpressionAsync()`: Query using LINQ expressions
  - `AddAndSaveAsync()`: Create new entities
  - `UpdateAndSaveAsync()`: Update existing entities
  - `RemoveAndSaveAsync()`: Delete entities

  Uses `NuvTools.Data.EntityFrameworkCore.Extensions` for database operations and `NuvTools.Common.ResultWrapper.IResult` for operation results.

#### Extensions (`NuvTools.AspNetCore.EntityFrameworkCore.Extensions`)
- **ApplicationBuilderExtensions**: Contains `DatabaseMigrate<TContext>()` extension method for running EF Core migrations during application startup with optional timeout configuration.

### Base Class Hierarchy

When creating services:
1. Use `ServiceWithMapperBase<TDTO, TEntity>` for services that need AutoMapper but no database access
2. Use `ServiceWithCrudBase<TContext, TDTO, TEntity, TKey>` for services with full CRUD operations

Both classes use primary constructors with dependency injection.

## Code Style and Conventions

- **Nullable reference types** are enabled (`<Nullable>enable</Nullable>`)
- **Implicit usings** are enabled (`<ImplicitUsings>enable</ImplicitUsings>`)
- **Code style enforcement** is enabled during build (`<EnforceCodeStyleInBuild>True</EnforceCodeStyleInBuild>`)
- **XML documentation generation** is required (`<GenerateDocumentationFile>True</GenerateDocumentationFile>`)
- Check `.editorconfig` for additional style rules

## Dependencies

### NuvTools.AspNetCore
- AutoMapper 16.0.0
- Microsoft.Extensions.Localization.Abstractions 10.0.0
- Microsoft.Extensions.Logging.Abstractions 10.0.0
- Microsoft.JSInterop (version varies by target framework)

### NuvTools.AspNetCore.EntityFrameworkCore
- NuvTools.Data.EntityFrameworkCore 9.5.0
- Microsoft.EntityFrameworkCore.Relational 9.0.10
- Microsoft.AspNetCore.App (framework reference)
- NuvTools.AspNetCore (project reference)

## Strong-Name Signing

Both libraries use strong-name signing:
- NuvTools.AspNetCore uses `NuvTools.AspNetCore.snk`
- NuvTools.AspNetCore.EntityFrameworkCore uses `NuvTools.AspNetCore.EntityFrameworkCore.snk`

## Testing

Tests use NUnit framework (version 4.4.0) with NUnit3TestAdapter. The test project targets only `net10.0`.
