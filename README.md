[![Discord Server](https://badgen.net/badge/discord/join%20chat/7289DA?icon=discord)](https://discord.gg/psrCP35kdk)
[![Download](https://img.shields.io/badge/nuget-download-blue)](https://www.nuget.org/packages/PixiEditor.Parser/)
[![Downloads](https://img.shields.io/nuget/dt/PixiEditor.Parser)](https://www.nuget.org/packages/PixiEditor.Parser/)

<img src="https://user-images.githubusercontent.com/45312141/102829812-2e1c1c80-43e8-11eb-889c-0043e66e5fe5.png" width="700" />

---

## Getting started

Use `PixiParser.Deserialize()` to deserialize a document and `PixiParser.Serialize()` to serialize

```cs
using PixiEditor.Parser;


Document document = PixiParser.Deserialize("./pixiFile.pixi");

// Do some stuff with the document

PixiParser.Serialize(document, "./pixiFile.pixi");
```

## Installation

Package Manager Console:
```
Install-Package PixiEditor.Parser
```

.NET CLI:
```
dotnet add package PixiEditor.Parser
```

## SkiaSharp

We provide a package containing extensions for working with [SkiaSharp](https://github.com/mono/SkiaSharp)

### Example Usage

```cs
using PixiEditor.Parser.Skia;

// Get a SKImage from the png data of a IImageContainer (e.g. ImageLayer or ReferenceLayer)
SKImage image = layer.ToSKImage();
```

```cs
using PixiEditor.Parser.Skia;

// Encode the image data of the SKImage into the png data of a IImageContainer (e.g. ImageLayer or ReferenceLayer)
layer.FromSKImage(image);
```

### Installation

Package Manager Console:
```
Install-Package PixiEditor.Parser.Skia
```

.NET CLI:
```
dotnet add package PixiEditor.Parser.Skia
```

## Need Help?

You can find support here:

* Ask on our [Discord](https://discord.gg/qSRMYmq)
* Open a [Issue](https://github.com/PixiEditor/PixiParser/issues/new)

