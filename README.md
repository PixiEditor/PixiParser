<img src="https://user-images.githubusercontent.com/45312141/102829812-2e1c1c80-43e8-11eb-889c-0043e66e5fe5.png" width="700" />

---

## Getting started

Use `PixiParser.Deserialize()` to deserialize a document and `PixiParser.Serialize()` to serialize

```cs
SerializableDocument document = PixiParser.Deserialize("./pixiFile.pixi");

// Do some stuff with the document

PixiParser.Serialize(document, "./pixiFile.pixi");
```
