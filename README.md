# PixiParser
A parser for serializing and deserializing .pixi files used by [PixiEditor](https://github.com/PixiEditor/PixiEditor)

## Getting started

Use `PixiParser.Deserialize()` to deserialize a document and `PixiParser.Serialize()` to serialize

```cs
SerializableDocument document = PixiParser.Deserialize("./pixiFile.pixi");

// Do some stuff with the document

PixiParser.Serialize(document, "./pixiFile.pixi");
```
