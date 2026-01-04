using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using System.Collections.Generic;
using System.IO;

namespace Engine;

public static class Global
{
    public const int DEFAULT_FPS = 60;
    public const int RESOLUTION_X = 480;
    public const int RESOLUTION_Y = 270;
    public const int DEFAULT_WINDOW_RESOLUTION_X = 1920;
    public const int DEFAULT_WINDOW_RESOLUTION_Y = 1080;

    public static readonly Color BACKGROUND_COLOUR = Color.CornflowerBlue;

    public static float DELTA_TIME {get; private set;} = -1f;

    public static GraphicsDeviceManager Graphics {get; private set;}
    public static SpriteBatch Batch {get; private set;}
    public static Random Rng {get; private set;}

    public static MouseState M_State {get; private set;}
    public static MouseState LastMouse {get; private set;}
    public static KeyboardState K_State {get; private set;}
    public static KeyboardState LastKeys {get; private set;}

    public static Texture2D PixelTexture {get; private set;}

    private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
    private static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();
    private static Dictionary<string, Song> songs = new Dictionary<string, Song>();

    public static void Load(GraphicsDeviceManager graphics, SpriteBatch batch, string assetsPath)
    {
        Graphics = graphics;
        Batch = batch;
        PixelTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
        PixelTexture.SetData(new[] { Color.White });
        Rng = new Random();

        loadTextures(assetsPath);
        loadSounds(assetsPath);
        loadSongs(assetsPath);
    }

    private static void loadTextures(string assetsPath)
    {
        const string png = ".png", jpg = ".jpg";
        var device = Graphics.GraphicsDevice;
        var texturePath = assetsPath + "/png/";
        var textureFiles = Directory.GetFiles(texturePath);
        foreach (var file in textureFiles)
        {
            if (!file.EndsWith(png) && !file.EndsWith(jpg))
            {
                throw new Exception("File not png or jpg");
            }
            textures.Add(file.Replace(texturePath, ""), Texture2D.FromFile(device, file));
        }
    }

    private static void loadSounds(string assetsPath)
    {
        const string wav = ".wav";
        var soundPath = assetsPath + "/audio/sfx/";
        var soundFiles = Directory.GetFiles(soundPath);
        foreach (var file in soundFiles)
        {
            if (!file.EndsWith(wav))
            {
                throw new Exception("File not wav");
            }

            sounds.Add(file.Replace(soundPath, ""), SoundEffect.FromFile(file));
        }
    }

    private static void loadSongs(string assetsPath)
    {
        const string ogg = ".ogg";
        var songPath = assetsPath + "/audio/song/";
        var songFiles = Directory.GetFiles(songPath);
        foreach (var file in songFiles)
        {
            if (!file.EndsWith(ogg))
            {
                throw new Exception("File not ogg");
            }

            var fileName = file.Replace(songPath, "");
            songs.Add(fileName, Song.FromUri(fileName, new Uri(file)));
        }
    }

    public static void Unload()
    {
        PixelTexture.Dispose();
        Batch.Dispose();
        Graphics.Dispose();
        foreach (var tex in textures)
        {
            tex.Value.Dispose();
        }
    }

    public static void Update(float dt)
    {
        DELTA_TIME = dt;
        LastMouse = M_State;
        LastKeys = K_State;
        M_State = Mouse.GetState();
        K_State = Keyboard.GetState();
    }

    public static Texture2D GetTexture(string key)
    {
        return textures[key];
    }

    public static SoundEffect GetSound(string key)
    {
        return sounds[key];
    }

    public static Song GetSong(string key)
    {
        return songs[key];
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