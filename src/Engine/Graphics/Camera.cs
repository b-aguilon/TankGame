namespace Engine.Graphics;

partial class Renderer
{
    private Matrix camera;

    private static Vector2 centerCameraOffset {get;} = new Vector2(-Global.RESOLUTION_X / 2, -Global.RESOLUTION_Y / 2);

    public Vector2 CameraPos {get; set;} = Vector2.Zero;

    public void CameraFollow(Vector2 pos, float lag = 1f)
    {
        if (lag <= 0f)
        {
            Console.WriteLine("ERROR: Camera lag cannot be less than 0");
            lag = 1f;
        }

        var moveX = (pos.X - CameraPos.X) / lag * Global.DELTA_TIME * Global.DEFAULT_FPS;
        var moveY = (pos.Y - CameraPos.Y) / lag * Global.DELTA_TIME * Global.DEFAULT_FPS;

        CameraPos = new Vector2(CameraPos.X + moveX, CameraPos.Y + moveY);

        camera =
        Matrix.CreateTranslation((int)-CameraPos.X - (int)centerCameraOffset.X, (int)-CameraPos.Y - (int)centerCameraOffset.Y, 0f);
    }

    public Vector2 GetCameraOffsetTowardPoint(Vector2 currentOffset, Vector2 targetPoint, int maxDistance)
    {
        var dt = Global.DELTA_TIME;
        var fps = Global.DEFAULT_FPS;
        var dir = targetPoint - CameraPos;

        var offset = currentOffset;
        offset.X += dir.X * dt * fps;
        offset.Y += dir.Y * dt * fps;

        var normalizedDir = Vector2.Normalize(dir);
        var bound = normalizedDir * maxDistance;
        if (offset.X < bound.X && bound.X < 0)
        {
            offset.X = bound.X;
        }
        else if (offset.X > bound.X && bound.X > 0)
        {
            offset.X = bound.X;
        }
        if (offset.Y < bound.Y && bound.Y < 0)
        {
            offset.Y = bound.Y;
        }
        else if (offset.Y > bound.Y && bound.Y > 0)
        {
            offset.Y = bound.Y;
        }

        return offset;
    }
}