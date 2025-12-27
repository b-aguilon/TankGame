using Engine.Collisions;

namespace Engine.Graphics;

partial class Renderer
{
    public static void DrawEntity(Entity entity)
    {
        var rect = entity.Destination;
        var pivot = entity.RotationPivot;
        var adjustedDestination = 
        new Rectangle
        (
            rect.X + (int)entity.DrawOffset.X - (int)pivot.X,
            rect.Y + (int)entity.DrawOffset.Y - (int)pivot.Y, 
            rect.Size.X, rect.Size.Y
        );

        Global.Batch.Draw
        (
            entity.Texture, 
            adjustedDestination, 
            entity.Source, 
            entity.Tint, 
            entity.Rotation, 
            entity.RotationPivot, 
            SpriteEffects.None, 
            0f
        );
    }

    public static void DrawRectangle(Rectangle rect, Color colour, Vector2? offset = null)
    {
        var notNull = Vector2.Zero;
        if (offset is not null)
            notNull = (Vector2)offset;
        var offsetRect = 
        new Rectangle(rect.X + (int)notNull.X, rect.Y + (int)notNull.Y, rect.Width, rect.Height);
        Global.Batch.Draw(Global.PixelTexture, offsetRect, colour);
    }

    public static void DrawRectangle(RectCollider rect, Color colour, Vector2? offset = null)
    {
        var notNull = Vector2.Zero;
        if (offset is not null)
            notNull = (Vector2)offset;
        Global.Batch.Draw
        (
            Global.PixelTexture,
            new Rectangle(rect.Position.ToPoint() + notNull.ToPoint(), rect.Size.ToPoint()),
            colour
        );
    }

    public static void DrawLine(Vector2 start, Vector2 end, Color colour)
    {
        int length = (int)Vector2.Distance(start, end);
        Vector2 dir = end - start;
        var rotation = MathF.Atan2(dir.Y, dir.X);
        var line = new Rectangle((int)start.X, (int)start.Y, length, 1);

        Global.Batch.Draw
        (
            Global.PixelTexture,
            line,
            new(0, 0, 1, 1),
            colour,
            rotation,
            Vector2.Zero,
            SpriteEffects.None,
            0f
        );
    }
}