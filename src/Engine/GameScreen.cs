using Engine.Graphics;

using System.Collections.Generic;

namespace Engine;

abstract class GameScreen
{
    public event EventHandler<GameScreen> OnScreenChanged;

    public GameScreen() {}

    protected void changeScreen(GameScreen next) => OnScreenChanged?.Invoke(this, next);

    private void onEntityAdd(object entity, List<Entity> entities)
    {
        if (entity is not Entity)
        {
            throw new ArgumentException($"{entity.GetType()}");
        }

        entities.Add((Entity)entity);
        entityAddedEffects((Entity)entity);
    }

    private void onEntityRemove(object entity, List<Entity> entities)
    {
        if (entity is not Entity)
        {
            throw new ArgumentException($"{entity.GetType()}");
        }

        entities.Remove((Entity)entity);
        entityRemovedEffects((Entity)entity);
    }

    protected virtual void entityAddedEffects(Entity entity) {}

    protected virtual void entityRemovedEffects(Entity entity) {}

    public void Load()
    {
        Entities.AddEntityAddedListener(onEntityAdd);
        Entities.AddEntityRemovedListener(onEntityRemove);
        loadGameScreen();
    }

    protected virtual void loadGameScreen() { }

    public void Unload()
    {
        Entities.ClearEntityAddedListeners();
        Entities.ClearEntityRemovedListeners();
        foreach (var ent in Entities.GetEntities())
        {
            ent.Texture.Dispose();
        }
        Entities.ClearEntities();
        unloadGameScreen();
    }

    protected virtual void unloadGameScreen() { }

    public virtual void Update() { }

    public virtual void Draw() { }
}

class TestScreen : GameScreen
{
    private const int CAM_SPEED = 600;

    private Texture2D testBG;
    private Vector2 cameraPos;

    protected override void loadGameScreen()
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

    protected override void unloadGameScreen()
    {
        testBG.Dispose();
    }
}