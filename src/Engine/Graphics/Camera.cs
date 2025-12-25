namespace Engine.Graphics;

partial class Renderer
{
    private Matrix camera;
    private Vector2 camOffset = new Vector2(-Global.RESOLUTION_X / 2, -Global.RESOLUTION_Y / 2);
    private Vector2 cameraPos = Vector2.Zero;

    public void CameraFollow(Vector2 pos, float lag = 1f)
    {
        if (lag <= 0f)
        {
            Console.WriteLine("ERROR: Camera lag cannot be less than 0");
            lag = 1f;
        }

        var moveX = (pos.X - cameraPos.X) / lag * Global.DELTA_TIME * Global.DEFAULT_FPS;
        var moveY = (pos.Y - cameraPos.Y) / lag * Global.DELTA_TIME * Global.DEFAULT_FPS;

        cameraPos.X += moveX;
        cameraPos.Y += moveY;

        camera =
        Matrix.CreateTranslation((int)-cameraPos.X - (int)camOffset.X, (int)-cameraPos.Y - (int)camOffset.Y, 0f);
    }
}