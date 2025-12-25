namespace Engine;

public static class Global
{
    public const int DEFAULT_FPS = 60;
    public const int RESOLUTION_X = 320;
    public const int RESOLUTION_Y = 180;
    public const int DEFAULT_WINDOW_RESOLUTION_X = 1920;
    public const int DEFAULT_WINDOW_RESOLUTION_Y = 1080;
    public const string ASSETS_PATH = @"assets/";

    public static readonly Color BACKGROUND_COLOUR = Color.CornflowerBlue;

    public static float DELTA_TIME {get; private set;} = -1f;

    public static GraphicsDeviceManager Graphics {get; private set;}
    public static SpriteBatch Batch {get; private set;}

    public static MouseState M_State {get; private set;}
    public static MouseState LastMouse {get; private set;}
    public static KeyboardState K_State {get; private set;}
    public static KeyboardState LastKeys {get; private set;}

    public static Texture2D PixelTexture {get; private set;}

    public static void Load(GraphicsDeviceManager graphics, SpriteBatch batch)
    {
        Graphics = graphics;
        Batch = batch;
        PixelTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
        PixelTexture.SetData(new[] { Color.White });
    }

    public static void Unload()
    {
        PixelTexture.Dispose();
        Batch.Dispose();
        Graphics.Dispose();
    }

    public static void Update(float dt)
    {
        DELTA_TIME = dt;
        LastMouse = M_State;
        LastKeys = K_State;
        M_State = Mouse.GetState();
        K_State = Keyboard.GetState();
    }

    public static bool LeftMouseClicked()
    {
        return M_State.LeftButton == ButtonState.Pressed && LastMouse.LeftButton == ButtonState.Released;
    }

    public static bool RightMouseClicked()
    {
        return M_State.RightButton == ButtonState.Pressed && LastMouse.RightButton == ButtonState.Released;
    }

    public static float WrapRotation(float rotationRad)
    {
        var result = rotationRad % MathHelper.TwoPi;
        if (rotationRad < 0f)
        {
            result += MathHelper.TwoPi;
        }
        return result; 
    }
}