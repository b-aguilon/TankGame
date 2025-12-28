namespace Engine;

public abstract class Entity
{
    public Texture2D Texture;
    public Color Tint = Color.White;
    public Rectangle Source;
    public Rectangle Destination => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
    public Vector2 Position = Vector2.Zero;
    public Vector2 RotationPivot = Vector2.Zero;
    public Vector2 DrawOffset = Vector2.Zero;
    public float Rotation = 0f;
    public float LayerDepth = 0f;
    public int Width = 1, Height = 1;
}