global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using Microsoft.Xna.Framework.Input;

global using System;
global using System.Linq;

using Engine;
using Engine.Graphics;

namespace test;

public class MainGame : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private GameScreen currScreen;

    public MainGame()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        graphics.PreferredBackBufferWidth = 1920;
        graphics.PreferredBackBufferHeight = 1080;
        graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        Global.Load(graphics, spriteBatch);
        changeScreen(new Script.MainScreen());
    }

    protected override void UnloadContent()
    {
        Global.Unload();
        base.UnloadContent();
    }

    protected override void Update(GameTime gt)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        var dt = (float)gt.ElapsedGameTime.TotalSeconds;
        Global.Update(dt);
        currScreen.Update();
        Renderer.Get().Update();
        base.Update(gt);
    }

    protected override void Draw(GameTime gt)
    {
        currScreen.Draw();
        base.Draw(gt);
    }

    private void changeScreen(GameScreen next)
    {
        currScreen?.Unload();
        currScreen = next;
        currScreen.Load();
        currScreen.OnScreenChanged += (sender, state) => changeScreen(state);
    }
}
