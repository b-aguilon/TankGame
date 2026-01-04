namespace Engine.Graphics;

partial class Renderer
{
    private static Renderer instance;

    private readonly RenderTarget2D target;

    private float scale;
    private Vector2 blackBarsOffset;
    private Rectangle destinationRect;
    private SamplerState samplerState;

    static Renderer()
    {
        instance = new Renderer();
    }

    public static Renderer Get() => instance;

    private Renderer()
    {
        scale = 1f;
        blackBarsOffset = Vector2.Zero;
        target = new RenderTarget2D(Global.Graphics.GraphicsDevice, Global.RESOLUTION_X, Global.RESOLUTION_Y);
        samplerState = SamplerState.PointClamp;
    }

    public void Update()
    {
        var targetW = target.Width;
        var targetH = target.Height;
        var screenW = Global.Graphics.GraphicsDevice.Viewport.Width;
        var screenH = Global.Graphics.GraphicsDevice.Viewport.Height;

        float gameAspectRatio = (float)targetW / targetH;
        float actualAspectRatio = (float)screenW / screenH;
        float x = 0f, y = 0f, w = screenW, h = screenH;

        if (gameAspectRatio < actualAspectRatio)
        {
            w = h * gameAspectRatio;
            x = (screenW - w) / 2f;
            blackBarsOffset.X = x;
            scale = screenH / (float)targetH;
        }
        else
        {
            h = w / gameAspectRatio;
            y = (screenH - h) / 2f;
            blackBarsOffset.Y = y;
            scale = screenW / (float)targetW;
        }

        destinationRect = new Rectangle((int)x, (int)y, (int)w, (int)h);
    }

    public void Draw(Action<Renderer> drawWorld, Action<Renderer> drawScreen)
    {
        var targetW = target.Width;
        var targetH = target.Height;
        var batch = Global.Batch;

        Global.Graphics.GraphicsDevice.SetRenderTarget(target);
        Global.Graphics.GraphicsDevice.Clear(Global.BACKGROUND_COLOUR);
        batch.Begin(SpriteSortMode.Deferred, null, samplerState, null, null,  null, camera);
        drawWorld?.Invoke(this);
        batch.End();
        batch.Begin(SpriteSortMode.Deferred, null, samplerState);
        drawScreen?.Invoke(this);
        batch.End();
        Global.Graphics.GraphicsDevice.SetRenderTarget(null);

        batch.Begin(SpriteSortMode.Deferred, null, samplerState);
        batch.Draw(target, destinationRect, new Rectangle(0, 0, target.Width, target.Height), Color.White);
        batch.End();
    }

    public void ToggleFullscreen()
    {
        if (Global.Graphics.IsFullScreen)
        {
            Global.Graphics.PreferredBackBufferWidth = Global.DEFAULT_WINDOW_RESOLUTION_X;
            Global.Graphics.PreferredBackBufferHeight = Global.DEFAULT_WINDOW_RESOLUTION_Y;
            Global.Graphics.ApplyChanges();
        }
        else
        {
            var display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            int width = display.Width;
            int height = display.Height;

            Global.Graphics.PreferredBackBufferWidth = width;
            Global.Graphics.PreferredBackBufferHeight = height;
            Global.Graphics.ApplyChanges();
        }

        Global.Graphics.ToggleFullScreen();
    }

    public Vector2 GetWorldMousePos()
    {
        var mousePos = Global.M_State.Position.ToVector2();
        return (mousePos - blackBarsOffset) / scale + CameraPos + centerCameraOffset;
    }

    public Vector2 GetScreenMousePos()
    {
        var mousePos = Global.M_State.Position.ToVector2(); 
        return (mousePos - blackBarsOffset) / scale;
    }
}