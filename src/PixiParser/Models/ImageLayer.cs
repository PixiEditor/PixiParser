using PixiEditor.Parser.Helpers;

namespace PixiEditor.Parser;

public sealed class ImageLayer : IImageContainer, IMaskable
{
    private float _opacity = 1;
    
    public byte[] ImageBytes { get; set; }
    
    public byte[] MaskBytes { get; set; }
    
    public bool Enabled { get; set; }

    public float Opacity
    {
        get => _opacity;
        set => this.SetOpacity(ref _opacity, value);
    }
}