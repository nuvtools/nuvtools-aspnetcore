# NuvTools ASP.NET Core Libraries

[![NuGet](https://img.shields.io/nuget/v/NuvTools.AspNetCore.svg)](https://www.nuget.org/packages/NuvTools.AspNetCore/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A suite of helper libraries designed to simplify and enhance ASP.NET Core application development. These libraries target modern .NET platforms, including .NET 8, .NET 9, and .NET 10.

## Libraries

### NuvTools.AspNetCore

A general-purpose library providing common helpers for ASP.NET Core applications.

**Key Features:**
- **Composite Localization**: Multi-source localization with prefix-based routing and fallback resolution
- **AutoMapper Integration**: Base classes for services with built-in DTO/Entity mapping
- **JavaScript Interop**: Clipboard operations for Blazor applications
- Fully documented with XML comments for IntelliSense support

### NuvTools.AspNetCore.EntityFrameworkCore

A specialized library offering Entity Framework Core helpers for ASP.NET Core projects.

**Key Features:**
- **CRUD Base Classes**: Pre-built service base classes with complete CRUD operations
- **Database Migration Extensions**: Automatic migration application with configurable timeouts
- **AutoMapper + EF Integration**: Seamless DTO/Entity conversion with database operations
- Built on top of `NuvTools.AspNetCore` for maximum code reuse

## Installation

Install via NuGet Package Manager:

```bash
# For general ASP.NET Core helpers
dotnet add package NuvTools.AspNetCore

# For Entity Framework Core helpers (includes NuvTools.AspNetCore)
dotnet add package NuvTools.AspNetCore.EntityFrameworkCore
```

Or via Package Manager Console:

```powershell
Install-Package NuvTools.AspNetCore
Install-Package NuvTools.AspNetCore.EntityFrameworkCore
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

### AutoMapper Service Base

Create services with built-in mapping:

```csharp
public class ProductService : ServiceWithMapperBase<ProductDto, Product>
{
    public ProductService(IMapper mapper) : base(mapper) { }

    public ProductDto Convert(Product product)
    {
        return ConvertToDTO(product);
    }

    public IEnumerable<ProductDto> ConvertAll(IEnumerable<Product> products)
    {
        return ConvertToDTO(products);
    }
}
```

### CRUD Service Base

Create full CRUD services with minimal code:

```csharp
public class CustomerService : ServiceWithCrudBase<MyDbContext, CustomerDto, Customer, int>
{
    public CustomerService(MyDbContext context, IMapper mapper)
        : base(context, mapper) { }

    // FindAsync, AddAndSaveAsync, UpdateAndSaveAsync, RemoveAndSaveAsync
    // are already implemented!

    public async Task<CustomerDto?> GetCustomerById(int id)
    {
        return await FindAsync(id);
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
- **Strong-named assemblies**: Signed for use in fully trusted environments
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
- AutoMapper 16.0.0+

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Links

- [GitHub Repository](https://github.com/nuvtools/nuvtools-aspnetcore)
- [NuGet Package - NuvTools.AspNetCore](https://www.nuget.org/packages/NuvTools.AspNetCore/)
- [NuGet Package - NuvTools.AspNetCore.EntityFrameworkCore](https://www.nuget.org/packages/NuvTools.AspNetCore.EntityFrameworkCore/)
- [Official Website](https://nuvtools.com)