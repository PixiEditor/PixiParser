using System;

namespace PixiEditor.Parser;

public interface ISize<T> : IStructureMember where T : unmanaged
{
    public T Width { get; set; }
    
    public T Height { get; set; }

    public T OffsetX { get; set; }
    
    public T OffsetY { get; set; }
    
#if NET5_0_OR_GREATER
    public Type GetSizeType() => typeof(T);
#endif
}