using DotTiled;

using Engine;
using Engine.Collisions;
using Engine.Graphics;
using Engine.Tilemap;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace Script;

class MainScreen : GameScreen
{
    private const int BARREL_LENGTH = 15;
    private const int CAMERA_LAG = 15;
    private const int CAM_MAX_OFFSET = 100;

    private Player player;
    private MapData mapData;
    private Vector2 cameraOffset = Vector2.Zero;

    private readonly List<SoundEffectInstance> soundInstances = new List<SoundEffectInstance>();
    private readonly List<RectCollider> colliders = new List<RectCollider>();

    protected override void loadGameScreen()
    {
        GameEntities.AddShootListener(onEntityShoot);

        var map = Levels.Loader.LoadMap("assets/map/level/1.tmx");
        mapData = new MapData
        {
            Map = map,
            VisualLayers = map.Layers.OfType<Group>().Single(l => l.Name == "Visuals").Layers.OfType<TileLayer>(),
            CollisionLayer = map.Layers.OfType<ObjectLayer>().Single(l => l.Name == "Collisions"),
            SpawnLayer = map.Layers.OfType<ObjectLayer>().Single(l => l.Name == "Spawn"),
            TilesetTextures = Levels.LoadTilesetTextures(map)  
        };
        
        foreach (var rect in mapData.CollisionLayer.Objects.OfType<RectangleObject>())
        {
            colliders.Add(new RectCollider(new(rect.X, rect.Y), new(rect.Width, rect.Height)));
        }

        foreach (var spawn in mapData.SpawnLayer.Objects.OfType<PointObject>())
        {
            Enemy enemy;
            bool playerSpawned = false;
            switch (spawn.Name)
            {
                case "Camera":
                    Renderer.Get().CameraPos = new(spawn.X, spawn.Y);
                    break;
                case "Player":
                    if (playerSpawned)
                    {
                        throw new Exception("Muliple player spawns");
                    }
                    this.player = GameEntities.MakePlayer(new(spawn.X, spawn.Y));
                    Entities.TriggerAddEntity(player);
                    playerSpawned = true;
                    break;
                case "Moving":
                    enemy = GameEntities.MakeEnemyNormal(new(spawn.X, spawn.Y));
                    Entities.TriggerAddEntity(enemy);
                    break;
                case "Stationary":
                    enemy = GameEntities.MakeEnemyStationary(new(spawn.X, spawn.Y));
                    Entities.TriggerAddEntity(enemy);
                    break;
                case "Fast":
                    enemy = GameEntities.MakeEnemyFast(new(spawn.X, spawn.Y));
                    Entities.TriggerAddEntity(enemy);
                    break;
                default:
                    break;
            }
        }
    }

    private void onEntityShoot(object sender, EventArgs e)
    {
        if (sender is not Entity || sender is not TankData tank)
            throw new Exception("Only tank entities can shoot");
        
        const string noTexture = "none";
        const string defaultTexture = "shell.png";
        Shell shell;
        var entity = (Entity)sender; 
        var shellTexture = noTexture;
        var shootSound = Global.GetSound("shoot1.wav").CreateInstance();

        switch (sender)
        {
            case Enemy enemy:
                shootSound.Volume = GameEntities.ENEMY_SHOOT_VOLUME;
                if (enemy.EnemyType == EnemyType.Fast || enemy.EnemyType == EnemyType.Stationary)
                    shellTexture = "shellFast.png";
                break;
            case Player:
                shootSound.Volume = GameEntities.PLAYER_SHOOT_VOLUME;
                break;
            default:
                throw new NotImplementedException($"{sender.GetType()}");
        }

        if (shellTexture == noTexture)
            shellTexture = defaultTexture;

        shootSound.Play();
        soundInstances.Add(shootSound);
        shell = GameEntities.MakeShell
        (
            entity.Position + tank.Barrel.Direction * BARREL_LENGTH,
            shellTexture, tank.Barrel.Rotation, 
            tank.ShellSpeed
        );
        shell.ShotBy = entity;
        Entities.TriggerAddEntity(shell);
    }

    protected override void entityAddedEffects(Entity entity)
    {
        switch (entity)
        {
            case TankData tank:
                Entities.TriggerAddEntity(tank.Barrel);
                break;
            default:
                break;
        }
    }

    protected override void entityRemovedEffects(Entity entity)
    {
        if (entity is TankData tank)
        {
            Entities.TriggerRemoveEntity(tank.Barrel);
        }
    }

    public override void Update()
    {
        var mouseWorldPos = Renderer.Get().GetWorldMousePos();

        if (Global.K_State.IsKeyDown(Keys.Enter) && Global.LastKeys.IsKeyUp(Keys.Enter))
        {
            changeScreen(new MainScreen());
        }
        if (Global.K_State.IsKeyDown(Keys.P) && Global.LastKeys.IsKeyUp(Keys.P))
        {
            Renderer.Get().ToggleFullscreen();
        }

        foreach (var ent in Entities.GetEntities().ToArray())
        {
            switch (ent)
            {
                case Player player:
                    if (Global.K_State.IsKeyDown(Keys.LeftShift))
                    {
                        cameraOffset = Renderer.Get().GetCameraOffsetTowardPoint
                        (
                            cameraOffset,
                            mouseWorldPos,
                            CAM_MAX_OFFSET
                        );
                    }
                    else
                    {
                        cameraOffset = Vector2.Zero;
                    }
                    PlayerController.UpdatePlayer(player);
                    break;
                case Shell shell:
                    ShellController.UpdateShell(shell, colliders);
                    break;
                case Enemy enemy:
                    EnemyController.UpdateEnemy(enemy, player.Position);
                    break;
                default:
                    break;
            }

            if (ent is TankData tank)
            {
                TankController.UpdateTank(ent, tank, colliders);
            }
        }

        Renderer.Get().CameraFollow(player.Position + cameraOffset, CAMERA_LAG);
        unloadFinishedSounds();
    }

    private void unloadFinishedSounds()
    {
        foreach (var instance in soundInstances.ToArray())
        {
            if (instance.State == SoundState.Stopped)
            {
                instance.Dispose();
                soundInstances.Remove(instance);
            }
        }
    }

    public override void Draw()
    {
        Renderer.Get().Draw( 
        (world) =>
        {
            Levels.DrawLayers(mapData, ["Ground"]);

            foreach (var ent in Entities.GetEntities().OrderBy(e => e.LayerDepth))
            {
                Renderer.DrawEntity(ent);
            }

            Levels.DrawLayers(mapData, ["Wall"]);
        }, 
        (screen) =>
        {
            
        });
    }

    protected override void unloadGameScreen()
    {
        GameEntities.ClearShootListeners();
    }
}