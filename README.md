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

Blazor services for browser APIs and common functionality.

**Key Features:**
- **Clipboard Service**: Read/write clipboard operations
- **Local Storage Service**: Browser localStorage with JSON serialization
- **Session Storage Service**: Browser sessionStorage with JSON serialization
- **Loading Service**: Counter-based loading indicator with nested call support and `RunAsync` helper
- **Download File Service**: Trigger browser file downloads from byte arrays, streams, or text with chunked transfer for large files
- **Confirm Exit Service**: Ask the browser to confirm before closing or reloading the tab when there are unsaved changes
- **Camera Component**: Live camera preview and frame capture, with configurable resolution, camera, mirroring and image format

### NuvTools.AspNetCore.Blazor.MudBlazor

MudBlazor components and utilities for Blazor applications.

**Key Features:**
- **Pattern Converters**: Flexible input masking for MudTextField (phone, documents, etc.)
- **Country-Specific Converters**: Pre-configured formatters for Brazil and United States
- **MudTable Base Class**: Server-side paging with session storage persistence and `SkipCount` mode support for large datasets

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

### Loading Service (Blazor)

Manage loading indicators with nested call support:

```csharp
// Register in Program.cs (or use services.AddBlazorServices() to register all)
services.AddLoadingService();

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

### Download File Service (Blazor)

Trigger browser file downloads from byte arrays, streams, or text:

```csharp
// Register in Program.cs (or use services.AddBlazorServices() to register all)
services.AddDownloadFileService();

// In your component
@inject IDownloadFileService DownloadFileService

@code {
    // Download from a byte array
    private async Task DownloadPdf()
    {
        byte[] pdfBytes = await GenerateReport();
        await DownloadFileService.DownloadFileAsync("report.pdf", pdfBytes, "application/pdf");
    }

    // Download from a stream (automatically chunked for large files)
    private async Task DownloadLargeFile()
    {
        using var stream = File.OpenRead("largefile.zip");
        await DownloadFileService.DownloadFileAsync("largefile.zip", stream, "application/zip");
    }

    // Download from text
    private async Task DownloadCsv()
    {
        var csv = "Name,Age\nAlice,30\nBob,25";
        await DownloadFileService.DownloadFileFromTextAsync(
            "data.csv", csv, Encoding.UTF8, "text/csv");
    }
}
```

### Confirm Exit Service (Blazor)

Ask the browser to confirm before the user closes or reloads the tab. Enable it only while there are
unsaved changes — both methods are idempotent, so they can be called on every render without extra
interop calls:

```csharp
// Register in Program.cs (or use services.AddBlazorServices() to register all)
services.AddConfirmExitService();

// In your component
@inject IConfirmExitService ConfirmExit

@code {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (HasUnsavedChanges())
            await ConfirmExit.EnableAsync();
        else
            await ConfirmExit.DisableAsync();
    }

    // The service is scoped, so turn the confirmation off when leaving the page.
    public async ValueTask DisposeAsync() => await ConfirmExit.DisableAsync();
}
```

The warning text comes from the browser and cannot be customized. It covers closing/reloading the tab
and leaving the application, but not in-app (SPA) navigation.

### Camera Component (Blazor)

Live camera preview and frame capture. The component owns its own `<video>`, capture `<canvas>` and
media stream — several instances can run side by side — and renders no layout of its own, so the page
keeps full control of the look:

```razor
@using NuvTools.AspNetCore.Blazor.Components

<div class="camera-frame">
    <NuvCamera @ref="_camera"
               FacingMode="CameraFacingMode.Environment"
               Mirrored="false"
               Width="1280" Height="720"
               Format="CameraImageFormat.Jpeg" Quality="0.9"
               Style="width:100%;height:100%;object-fit:cover"
               OnError="HandleCameraError">
        <Overlay>
            <div class="scan-guide"></div>
        </Overlay>
    </NuvCamera>
</div>

<button @onclick="ScanAsync">Scan</button>

@code {
    private NuvCamera _camera = default!;

    private async Task ScanAsync()
    {
        var capture = await _camera.CaptureAsync();
        if (capture is null) return;

        // Base64 already comes without the "data:image/jpeg;base64," prefix.
        await Reader.ReadAsync(capture.Base64);   // or capture.Bytes / capture.DataUrl
    }

    // NotAllowedError (denied), NotFoundError (no camera), NotReadableError (camera in use).
    private void HandleCameraError(CameraError error) => Message = Localizer[error.Name];
}
```

The camera starts on first render (`AutoStart`) and is released on dispose — every track is stopped, so
the camera light goes off. Use `StartAsync`/`StopAsync` to drive it manually, `GetDevicesAsync` to list
the available cameras and `SwitchDeviceAsync` to change camera. `Mirrored` applies to both the preview
and the captured frame, so what the user sees is what gets saved; capture defaults to the stream's
native resolution unless `CaptureWidth`/`CaptureHeight` are set.

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

### Server-Side Paging with MudTable

`MudTablePageBase` provides a base class for server-side paging with MudTable. It handles filter persistence, sort mapping, and loading indicators:

```csharp
public class UserListPage : MudTablePageBase<UserDto, UserFilter, UserSortColumn>
{
    [Inject] private IUserService UserService { get; set; } = default!;

    protected override string SessionStorageKey => "user-list-filter";

    protected override UserFilter CreateDefaultFilter() => new()
    {
        SortColumn = UserSortColumn.Name,
        PageSize = 25
    };

    protected override async Task<IResult<PagingWithEnumerableList<UserDto>>> FetchDataAsync(
        UserFilter filter, CancellationToken cancellationToken)
    {
        return await UserService.GetUsersAsync(filter, cancellationToken);
    }

    protected override UserSortColumn MapSortLabelToOrdering(string sortLabel)
        => Enum.Parse<UserSortColumn>(sortLabel);
}
```

**SkipCount mode for large datasets**: Set `CountMode = PagingCountMode.SkipCount` in your filter to skip the expensive `COUNT(*)` query. The base class automatically synthesizes `TotalItems` for MudTablePager so Next/Previous navigation works correctly:

```csharp
protected override UserFilter CreateDefaultFilter() => new()
{
    SortColumn = UserSortColumn.Name,
    PageSize = 25,
    CountMode = PagingCountMode.SkipCount // No COUNT(*) query
};
```

When using `SkipCount`, customize `MudTablePager` to hide the unknown total:

```razor
<MudTable ServerData="@(new Func<TableState, CancellationToken, Task<TableData<UserDto>>>(ServerReload))"
          @ref="Table" CurrentPage="CurrentPage">
    @* columns *@
    <PagerContent>
        <MudTablePager PageSizeOptions="@(new int[] { 25, 50, 100 })"
                       InfoFormat=@("{first_item}-{last_item}")
                       HidePageNumber="true" />
    </PagerContent>
</MudTable>
```

The base class also exposes `HasNextPage` and `IsFirstPage` properties, along with `NextPageAsync()` / `PreviousPageAsync()` methods for building custom navigation UI.

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