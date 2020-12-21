<img src="https://raw.githubusercontent.com/PixiEditor/Assets/main/Images/PixiParserLogoWithName.png?token=AKZWRDLL3CGHGOPVF2PLFH274ELKM"/>

---

## Getting started

Use `PixiParser.Deserialize()` to deserialize a document and `PixiParser.Serialize()` to serialize

```cs
SerializableDocument document = PixiParser.Deserialize("./pixiFile.pixi");

// Do some stuff with the document

PixiParser.Serialize(document, "./pixiFile.pixi");
```
