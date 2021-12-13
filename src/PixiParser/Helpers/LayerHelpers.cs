namespace PixiEditor.Parser.Helpers;

public static class LayerHelpers
{
    public static bool GetFinalVisibility(this SerializableLayer layer, SerializableDocument document) => document.Layers.GetFinalLayerVisibilty(layer);

    public static double GetFinalOpacity(this SerializableLayer layer, SerializableDocument document) => document.Layers.GetFinalLayerOpacity(layer);
}
