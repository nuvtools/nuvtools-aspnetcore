# NuvTools ASP.NET Core Libraries

[![NuGet](https://img.shields.io/nuget/v/NuvTools.AspNetCore.svg)](https://www.nuget.org/packages/NuvTools.AspNetCore/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A suite of helper libraries designed to simplify and enhance ASP.NET Core application development. These libraries target modern .NET platforms, including .NET 8, .NET 9, and .NET 10.

## Libraries

### NuvTools.AspNetCore

ASP.NET Core helpers including composite localization.

**Key Features:**
- **Composite Localization**: Multi-source localization with prefix-based routing and fallback resolution
- Fully documented with XML comments for IntelliSense support

### NuvTools.AspNetCore.EntityFrameworkCore

Entity Framework Core helpers for ASP.NET Core.

**Key Features:**
- **Database Migration Extensions**: Automatic migration application with configurable timeouts

### NuvTools.AspNetCore.Blazor

Blazor JavaScript interop services for browser APIs.

**Key Features:**
- **Clipboard Service**: Read/write clipboard operations
- **Local Storage Service**: Browser localStorage with JSON serialization
- **Session Storage Service**: Browser sessionStorage with JSON serialization

### NuvTools.AspNetCore.Blazor.MudBlazor

MudBlazor components and utilities for Blazor applications.

**Key Features:**
- **Loading Service**: Counter-based loading indicator with nested call support and `RunAsync` helper
- **Pattern Converters**: Flexible input masking for MudTextField (phone, documents, etc.)
- **Country-Specific Converters**: Pre-configured formatters for Brazil and United States
- **MudTable Base Class**: Server-side paging with session storage persistence

## Installation

Install via NuGet Package Manager:

```bash
# For general ASP.NET Core helpers
dotnet add package NuvTools.AspNetCore

# For Entity Framework Core helpers (includes NuvTools.AspNetCore)
dotnet add package NuvTools.AspNetCore.EntityFrameworkCore

# For Blazor JavaScript interop services
dotnet add package NuvTools.AspNetCore.Blazor

# For MudBlazor components and utilities
dotnet add package NuvTools.AspNetCore.Blazor.MudBlazor
```

Or via Package Manager Console:

```powershell
Install-Package NuvTools.AspNetCore
Install-Package NuvTools.AspNetCore.EntityFrameworkCore
Install-Package NuvTools.AspNetCore.Blazor
Install-Package NuvTools.AspNetCore.Blazor.MudBlazor
```

## Quick Start

### Composite Localization

Set up multiple localization sources with prefix-based routing:

```csharp
// In Program.cs or Startup.cs
services.AddCompositeLocalizer(
    namedResourceTypes: new Dictionary<string, Type>
    {
        ["Errors"] = typeof(ErrorResources),
        ["Messages"] = typeof(MessageResources)
    },
    unnamedResourceTypes: new[] { typeof(SharedResources) }
);

// In your code
public class MyService
{
    private readonly CompositeLocalizer _localizer;

    public MyService(CompositeLocalizer localizer)
    {
        _localizer = localizer;
    }

    public string GetMessage()
    {
        // Use prefix to target specific localizer
        var error = _localizer["Errors:ValidationFailed"];

        // Or let it search all localizers
        var message = _localizer["WelcomeMessage"];

        return message;
    }
}
```

### Database Migrations

Apply migrations automatically on startup:

```csharp
// In Program.cs
var app = builder.Build();

// Apply migrations with default timeout
app.DatabaseMigrate<MyDbContext>();

// Or with custom timeout
app.DatabaseMigrate<MyDbContext>(TimeSpan.FromMinutes(5));

app.Run();
```

### Loading Service (MudBlazor)

Manage loading indicators with nested call support:

```csharp
// Register in Program.cs
services.AddScoped<ILoadingService, LoadingService>();

// In your component
@inject ILoadingService LoadingService
@implements IDisposable

<MudOverlay Visible="LoadingService.IsLoading" DarkBackground="true">
    <MudProgressCircular Color="Color.Primary" Indeterminate="true" />
</MudOverlay>

@code {
    protected override void OnInitialized()
    {
        LoadingService.OnChange += StateHasChanged;
    }

    private async Task LoadData()
    {
        // Option 1: Manual Show/Hide (supports nesting)
        LoadingService.Show();
        try { await FetchData(); }
        finally { LoadingService.Hide(); }

        // Option 2: RunAsync helper (recommended)
        await LoadingService.RunAsync(async () => await FetchData());
    }

    public void Dispose() => LoadingService.OnChange -= StateHasChanged;
}
```

### Pattern Converters (MudBlazor)

Apply input masks to MudTextField components:

```csharp
@using NuvTools.AspNetCore.Blazor.MudBlazor.Converters

// Custom pattern: A=alphanumeric, N=numeric, L=letter
<MudTextField @bind-Value="LicensePlate"
              Converter="@(new PatternStringConverter("LLL-NANN"))" />
```

### Country-Specific Converters

**United States:**

```csharp
@using NuvTools.AspNetCore.Blazor.MudBlazor.UnitedStates.Converters

<MudTextField @bind-Value="Phone" Label="Phone"
              Converter="USDocumentConverters.Phone" />
// Output: (555) 123-4567

<MudTextField @bind-Value="Ssn" Label="SSN"
              Converter="USDocumentConverters.Ssn" />
// Output: 123-45-6789

<MudTextField @bind-Value="ZipCode" Label="ZIP Code"
              Converter="USDocumentConverters.ZipCode" />
// Output: 90210

// Format directly in code
var formatted = USDocumentConverters.FormatPhone("5551234567");
```

**Brazil:**

```csharp
@using NuvTools.AspNetCore.Blazor.MudBlazor.Brazil.Converters

<MudTextField @bind-Value="MobilePhone" Label="Mobile"
              Converter="BrazilianDocumentConverters.MobilePhone" />
// Output: (11) 99999-9999

<MudTextField @bind-Value="Cpf" Label="CPF"
              Converter="BrazilianDocumentConverters.Cpf" />
// Output: 123.456.789-00

// Auto-detect mobile vs landline
var formatted = BrazilianDocumentConverters.FormatPhone("11999999999");
```

### Clipboard Service (Blazor)

Use JavaScript interop for clipboard operations:

```csharp
@inject ClipboardService ClipboardService

<button @onclick="CopyToClipboard">Copy</button>
<button @onclick="PasteFromClipboard">Paste</button>

@code {
    private async Task CopyToClipboard()
    {
        await ClipboardService.WriteTextAsync("Hello, World!");
    }

    private async Task PasteFromClipboard()
    {
        var text = await ClipboardService.ReadTextAsync();
        Console.WriteLine(text);
    }
}
```

## Features

- **Multi-targeting**: Compatible with .NET 8, .NET 9, and .NET 10
- **Comprehensive documentation**: Full XML documentation for IntelliSense
- **Modular design**: Use only what you need
- **Best practices**: Promotes clean architecture patterns
- **Modern C# features**: Uses nullable reference types, implicit usings, and primary constructors

## Building from Source

This project uses the modern `.slnx` solution format (Visual Studio 2022 v17.11+).

```bash
# Clone the repository
git clone https://github.com/nuvtools/nuvtools-aspnetcore.git
cd nuvtools-aspnetcore

# Build the solution
dotnet build NuvTools.AspNetCore.slnx

# Run tests
dotnet test NuvTools.AspNetCore.slnx

# Create release packages
dotnet build NuvTools.AspNetCore.slnx --configuration Release
```

## Requirements

- .NET 8.0 SDK or higher
- Visual Studio 2022 (v17.11+) or Visual Studio Code with C# extension
- MudBlazor 8.0.0+ (for NuvTools.AspNetCore.Blazor.MudBlazor)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Links

- [GitHub Repository](https://github.com/nuvtools/nuvtools-aspnetcore)
- [NuGet Package - NuvTools.AspNetCore](https://www.nuget.org/packages/NuvTools.AspNetCore/)
- [NuGet Package - NuvTools.AspNetCore.EntityFrameworkCore](https://www.nuget.org/packages/NuvTools.AspNetCore.EntityFrameworkCore/)
- [NuGet Package - NuvTools.AspNetCore.Blazor](https://www.nuget.org/packages/NuvTools.AspNetCore.Blazor/)
- [NuGet Package - NuvTools.AspNetCore.Blazor.MudBlazor](https://www.nuget.org/packages/NuvTools.AspNetCore.Blazor.MudBlazor/)
- [Official Website](https://nuvtools.com)