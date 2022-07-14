using System;
using System.Collections.Generic;
using MessagePack;

namespace PixiEditor.Parser;

public static partial class PixiParser
{
    public static Version FileVersion { get; } = new(4, 0);
    
    public static Version MinSupportedVersion { get; } = new(4, 0);

    private static MessagePackSerializerOptions MessagePackOptions { get; } = MessagePackSerializerOptions.Standard
        .WithSecurity(MessagePackSecurity.UntrustedData)
        .WithResolver(Resolver.Instance);
    
    private static readonly byte[] Magic = { 20, 50, 49, 58, 49 };
    
    private const int MagicLength = 5;
    
    private const int HeaderLength = MagicLength + sizeof(int) * 4;
}
