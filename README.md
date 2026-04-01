# Pixault.Client

.NET SDK for the [Pixault](https://pixault.io) image processing CDN and API.

## Features

- **Fluent URL builder** — generate optimized image URLs with transforms (`width`, `height`, `fit`, `format`, `watermark`, etc.)
- **Upload client** — upload images and videos with folder support
- **Admin client** — manage images, folders, and metadata
- **ASP.NET Core integration** — `AddPixault()` DI extension for seamless setup

## Installation

```bash
dotnet add package Pixault.Client
```

## Quick Start

### Register services

```csharp
builder.Services.AddPixault(options =>
{
    options.BaseUrl = "https://img.pixault.io";
    options.Project = "my-project";
    options.ApiKey = builder.Configuration["Pixault:ApiKey"];
});
```

### Build image URLs

```csharp
@inject PixaultImageService ImageService

var url = ImageService.For("my-project", "image-id")
    .Width(800)
    .Height(600)
    .Fit(FitMode.Cover)
    .Format("webp")
    .Build();
```

### Generate SEO embed tags

```csharp
// Single <img> with auto-format negotiation and srcset
var imgTag = images.For("tattoo", "img_01JKABC")
    .Quality(85)
    .ToImgTag(alt: "Sunset tattoo", widths: [400, 800, 1200], sizes: "(max-width: 800px) 100vw, 800px");

// Full <picture> element with AVIF/WebP/JPEG sources
var pictureTag = images.For("tattoo", "img_01JKABC")
    .Quality(85)
    .ToPictureTag(alt: "Sunset tattoo", widths: [400, 800, 1200]);
```

### Upload an image

```csharp
@inject PixaultUploadClient UploadClient

using var stream = File.OpenRead("photo.jpg");
var result = await UploadClient.UploadAsync("my-project", "photo.jpg", stream, "image/jpeg");
Console.WriteLine(result.ImageId);
```

## Configuration

| Option | Description | Default |
|--------|-------------|---------|
| `BaseUrl` | Pixault CDN base URL | `https://img.pixault.io` |
| `Project` | Default project identifier | — |
| `ApiKey` | API key for authenticated operations | — |
| `HmacSecret` | HMAC secret for signed URLs | — |

## License

[MIT](LICENSE)
