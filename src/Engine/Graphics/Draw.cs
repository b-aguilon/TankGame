namespace Engine.Graphics;

partial class Renderer
{
    public static void DrawEntity(Entity entity)
    {
        Global.Batch.Draw
        (
            entity.Texture, 
            entity.Destination, 
            entity.Source, 
            entity.Tint, 
            entity.Rotation, 
            entity.Origin, 
            SpriteEffects.None, 
            0f
        );
    }

    public static void DrawRectangle(Rectangle rect, Color colour)
    {
        Global.Batch.Draw(Global.PixelTexture, rect, colour);
    }
}