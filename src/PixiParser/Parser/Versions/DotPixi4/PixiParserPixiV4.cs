using MessagePack;
using PixiEditor.Parser.Old.PixiV4;

namespace PixiEditor.Parser.Versions.DotPixi4;

internal partial class PixiParserPixiV4 : PixiParser<DocumentV4>
{
    private static MessagePackSerializerOptions MessagePackOptions { get; } = MessagePackSerializerOptions.Standard
        .WithSecurity(MessagePackSecurity.UntrustedData)
        .WithResolver(ResolverV4.Instance);
}