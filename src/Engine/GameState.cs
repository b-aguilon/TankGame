using Engine.Graphics;

namespace Engine;

abstract class GameState
{
    public event EventHandler<GameState> OnStateChanged;

    public GameState() {}

    protected void changeState(GameState next) => OnStateChanged?.Invoke(this, next);

    public virtual void Load() { }

    public virtual void Unload() { }

    public virtual void Update() { }

    public virtual void Draw() { }
}

class TestState : GameState
{
    private const int CAM_SPEED = 600;

    private Texture2D testBG;
    private Vector2 cameraPos;

    public override void Load()
    {
        testBG = Texture2D.FromFile(Global.Graphics.GraphicsDevice, "assets/png/test.png");
    }

    public override void Update()
    {
        var dt = Global.DELTA_TIME;
        var kState = Global.K_State;

        if (kState.IsKeyDown(Keys.W))
        {
            cameraPos.Y -= CAM_SPEED * dt;
        }
        else if (kState.IsKeyDown(Keys.S))
        {
            cameraPos.Y += CAM_SPEED * dt;
        }
        if (kState.IsKeyDown(Keys.A))
        {
            cameraPos.X -= CAM_SPEED * dt;
        }
        else if (kState.IsKeyDown(Keys.D))
        {
            cameraPos.X += CAM_SPEED * dt;
        }

        Renderer.Get().CameraFollow(cameraPos, 20);
    }

    public override void Draw()
    {
        Renderer.Get().Draw( 
        (renderer) =>
        {
            Global.Batch.Draw(testBG, Vector2.Zero, Color.White);
        }, 
        (renderer) =>
        {
        });
    }

    public override void Unload()
    {
        testBG.Dispose();
    }
}