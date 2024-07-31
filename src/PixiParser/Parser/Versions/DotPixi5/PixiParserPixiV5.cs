using MessagePack;

namespace PixiEditor.Parser.Versions.DotPixi5;

internal partial class PixiParserPixiV5 : PixiParser<Document>
{
    private static MessagePackSerializerOptions MessagePackOptions { get; } = MessagePackSerializerOptions.Standard
        .WithSecurity(MessagePackSecurity.UntrustedData)
        .WithResolver(ResolverV5.Instance);
}