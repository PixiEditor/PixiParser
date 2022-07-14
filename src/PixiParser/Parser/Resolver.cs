using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using PixiEditor.Parser.Collections;

namespace PixiEditor.Parser;

internal class Resolver : IFormatterResolver
{
    private readonly ColorCollectionFormatter _colorCollection = new();
    
    public static Resolver Instance { get; } = new();
    
    public IMessagePackFormatter<T> GetFormatter<T>()
    {
        if (typeof(T) == typeof(ColorCollection))
        {
            return (IMessagePackFormatter<T>)_colorCollection;
        }

        return StandardResolverAllowPrivate.Instance.GetFormatter<T>();
    }
}