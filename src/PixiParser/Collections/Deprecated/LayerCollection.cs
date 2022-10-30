using PixiEditor.Parser.Deprecated;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using PixiEditor.Parser.Helpers.Deprecated;

namespace PixiEditor.Parser.Collections.Deprecated;

[DebuggerDisplay("Count = {Count}")]
public class LayerCollection : IList<SerializableLayer>
{
    private readonly SerializableDocument _document;
    private readonly List<SerializableLayer> _layers;

    public SerializableLayer this[int index] { get => _layers[index]; set => _layers[index] = value; }

    public int Count => _layers.Count;

    public bool IsReadOnly => false;

    internal LayerCollection(SerializableDocument document)
    {
        _document = document;
        _layers = new List<SerializableLayer>();
    }

    internal LayerCollection(SerializableDocument document, IEnumerable<SerializableLayer> layers)
    {
        _document = document;
        _layers = new List<SerializableLayer>(layers);
    }

    internal LayerCollection(SerializableDocument document, List<SerializableLayer> layers)
    {
        _document = document;
        _layers = layers;
    }

    public void Add(SerializableLayer item)
    {
        _layers.Add(item);
    }

    public void Clear()
    {
        _layers.Clear();
        _document.ClearGroupLayerIndexes();
    }

    public bool Contains(SerializableLayer item) => _layers.Contains(item);

    public void CopyTo(SerializableLayer[] array, int arrayIndex)
    {
        for (int i = 0; i < _layers.Count; i++)
        {
            array[arrayIndex + i] = _layers[i];
        }
    }

    public IEnumerator<SerializableLayer> GetEnumerator() => _layers.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _layers.GetEnumerator();

    public int IndexOf(SerializableLayer item) => _layers.IndexOf(item);

    public void Insert(int index, SerializableLayer item)
    {
        _document.IncreaseGroupLayerIndexes(index);
        _layers.Insert(index, item);
    }

    public bool Remove(SerializableLayer item)
    {
        int index = _layers.IndexOf(item);
        if (index == -1) return false;

        RemoveAt(index);
        return true;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _layers.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        _document.DecreaseGroupLayerIndexes(index);
        _layers.RemoveAt(index);
    }

    public SerializableLayer Add(string name) => Add(name, 0, 0);

    public SerializableLayer Add(string name, int width, int height, bool isVisible = true, float opacity = 1, int offsetX = 0, int offsetY = 0)
    {
        SerializableLayer layer = new SerializableLayer(width, height, offsetX, offsetY) { IsVisible = isVisible, Opacity = opacity, Name = name };
        Add(layer);
        return layer;
    }

    /// <summary>
    /// Moves the <paramref name="layer"/> to the specified <paramref name="index"/>
    /// </summary>
    /// <param name="layer">The layer to move</param>
    /// <param name="index">The index where the <paramref name="layer"/> should be moved to</param>
    /// <returns>If the move operation was successfull (Returns false if the <paramref name="layer"/> couldn't be found)</returns>
    public bool MoveTo(SerializableLayer layer, int index)
    {
        int layerIndex = IndexOf(layer);

        if (layerIndex == -1 || index == -1)
        {
            return false;
        }

        RemoveAt(layerIndex);
        Insert(index, layer);

        return true;
    }

    /// <summary>
    /// Moves the <paramref name="toMove"/> layer below the <paramref name="target"/> layer
    /// </summary>
    /// <param name="toMove">The layer to move</param>
    /// <param name="target">The layer which should appear above the <paramref name="toMove"/> layer</param>
    /// <returns>If the move operation was successfull (Returns false if <paramref name="toMove"/> or <paramref name="target"/> couldn't be found)</returns>
    public bool MoveBelow(SerializableLayer toMove, SerializableLayer target)
    {
        return MoveTo(toMove, IndexOf(target));
    }

    /// <summary>
    /// Moves the <paramref name="toMove"/> layer above the <paramref name="target"/> layer
    /// </summary>
    /// <param name="toMove">The layer to move</param>
    /// <param name="target">The layer which should appear below the <paramref name="toMove"/> layer</param>
    /// <returns>If the move operation was successfull (Returns false if <paramref name="toMove"/> or <paramref name="target"/> couldn't be found)</returns>
    public bool MoveAbove(SerializableLayer toMove, SerializableLayer target)
    {
        return MoveTo(toMove, IndexOf(target) + 1);
    }

    /// <summary>
    /// Get's the opacity of <paramref name="layer"/> respecting the opacity of the parent groups
    /// </summary>
    public double GetFinalLayerOpacity(SerializableLayer layer)
    {
        if (layer.Opacity == 0)
        {
            return 0f;
        }

        double finalOpacity = layer.Opacity;
        var groups = GetGroups(layer);

        if (groups == null)
        {
            throw new InvalidOperationException("The layer was not found in the layer collection.");
        }

        foreach (var group in groups)
        {
            finalOpacity *= group.Opacity;
        }

        return finalOpacity;
    }

    public bool GetFinalLayerVisibilty(SerializableLayer layer)
    {
        if (!layer.IsVisible)
        {
            return false;
        }

        var groups = GetGroups(layer);

        if (groups == null)
        {
            throw new InvalidOperationException("The layer was not found in the layer collection.");
        }

        foreach (var group in groups)
        {
            if (!group.IsVisible)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Adds the <paramref name="layer"/> to the <paramref name="group"/>
    /// <para/><b>Note: All layers between the <paramref name="layer"/> and the <see cref="SerializableGroup.StartLayer"/> or <see cref="SerializableGroup.EndLayer"/> will be added to the group too.</b>
    /// </summary>
    public void AddToGroup(SerializableGroup group, SerializableLayer layer)
    {
        int index = IndexOf(layer);

        foreach (SerializableGroup groupStack in group.GetStack(_document))
        {
            groupStack.EnsureInside(index);
        }
    }

    /// <summary>
    /// Returns true if the <paramref name="layer"/> is inside the <paramref name="group"/>
    /// </summary>
    public bool ContainedIn(SerializableGroup group, SerializableLayer layer)
    {
        return ContainedIn(group, IndexOf(layer));
    }

    /// <summary>
    /// Get's the parents of the <paramref name="layer"/>
    /// </summary>
    /// <returns>Returns the hierachy of the parent groups or null when the layer wasn't found</returns>
    public IEnumerable<SerializableGroup> GetGroups(SerializableLayer layer)
    {
        _ = TryGetGroups(layer, out var parents);
        return parents;
    }

    /// <summary>
    /// Tries to get the <paramref name="parents"/> of the <paramref name="layer"/>
    /// </summary>
    /// <returns>True if the layer was found</returns>
    public bool TryGetGroups(SerializableLayer layer, out IEnumerable<SerializableGroup> parents)
    {
        int layerIndex = IndexOf(layer);

        if (layerIndex == -1)
        {
            parents = null;
            return false;
        }

        Stack<SerializableGroup> groups = new();

        GetGroups(layerIndex, _document.Groups, ref groups);

        parents = groups;
        return true;
    }

    private void GetGroups(int index, IEnumerable<SerializableGroup> subgroups, ref Stack<SerializableGroup> groups)
    {
        if (subgroups == null)
        {
            return;
        }

        foreach (SerializableGroup group in subgroups)
        {
            groups.Push(group);

            if (ContainedIn(group, index))
            {
                GetGroups(index, group.Subgroups, ref groups);
                return;
            }

            groups.Pop();
        }
    }

    private bool ContainedIn(SerializableGroup group, int layerIndex)
    {
        return group.SerializedStartLayer <= layerIndex && layerIndex <= group.SerializedEndLayer;
    }

    /// <summary>
    /// Gets the internal list instance
    /// </summary>
    internal List<SerializableLayer> GetList() => _layers;
}
