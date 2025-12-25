namespace Engine;

public abstract class Entity
{
    public Texture2D Texture;
    public Color Tint = Color.White;
    public Rectangle Source;
    public Rectangle Destination => new Rectangle((int)Position.X, (int)Position.Y, Width * (int)Scale.X, Height * (int)Scale.Y);
    public Vector2 Position = Vector2.Zero;
    public Vector2 Origin = Vector2.Zero;
    public Vector2 Scale = Vector2.One;
    public float Rotation = 0f;
    public int Width = 1, Height = 1;
}