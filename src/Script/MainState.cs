using Engine;
using Engine.Graphics;
using Engine.Tilemap;
using Engine.Collisions;

using DotTiled;
using System.Collections.Generic;

namespace Script;

class MainState : GameState
{
    private const int BARREL_LENGTH = 15;
    private const float SHELL_SPEED = 70;
    private const float SHELL_KILL_TIME_MAX = 10f;
    private const float ENEMY_STATIONARY_SHOOT_DELTA = 2f;

    private Player player;
    private MapData mapData;

    private readonly List<Entity> entities = new List<Entity>();
    private readonly List<RectCollider> colliders = new List<RectCollider>();

    public override void Load()
    {
        player = GameEntities.MakePlayer(new Vector2(100, 100), speed:45);
        var enemy = GameEntities.MakeTankStationary(new(200, 200));

        var map = Levels.Loader.LoadMap("assets/maps/world.tmx");
        mapData = new MapData
        {
            Map = map,
            VisualLayers = map.Layers.OfType<Group>().Single(l => l.Name == "Visuals").Layers.OfType<TileLayer>(),
            CollisionLayer = map.Layers.OfType<ObjectLayer>().Single(l => l.Name == "Collisions"),
            TilesetTextures = Levels.LoadTilesetTextures(map)  
        };
        foreach (var rect in mapData.CollisionLayer.Objects.OfType<RectangleObject>())
        {
            colliders.Add(new RectCollider(new(rect.X, rect.Y), new(rect.Width, rect.Height)));
        }

        entities.Add(player);
        entities.Add(player.TankData.Barrel);
        entities.Add(enemy);
        entities.Add(enemy.TankData.Barrel);
    }

    public override void Update()
    {
        var dt = Global.DELTA_TIME;

        updatePlayer(dt);

        foreach (var ent in entities.ToArray())
        {
            switch (ent)
            {
                case Shell shell:
                    updateShell(shell, dt);
                    break;
                case Enemy enemy:
                    updateEnemyStationary(enemy, dt);
                    break;
                default:
                    break;
            }
        }

        Renderer.Get().CameraFollow(player.Position);
    }

    private void updatePlayer(float dt)
    {
        var kState = Global.K_State;
        var mouseWorldPos = Renderer.Get().GetWorldMousePos();
        var shootDir = Vector2.Normalize(mouseWorldPos - player.TankData.Barrel.Position);

        if (kState.IsKeyDown(Keys.A) && kState.IsKeyDown(Keys.W))
        {
            player.TankData.TankDir = TankDir.UpLeft;
        }
        else if (kState.IsKeyDown(Keys.D) && kState.IsKeyDown(Keys.W))
        {
            player.TankData.TankDir = TankDir.UpRight;
        }
        else if (kState.IsKeyDown(Keys.A) && kState.IsKeyDown(Keys.S))
        {
            player.TankData.TankDir = TankDir.DownLeft;
        }
        else if (kState.IsKeyDown(Keys.D) && kState.IsKeyDown(Keys.S))
        {
            player.TankData.TankDir = TankDir.DownRight;
        }
        else if (kState.IsKeyDown(Keys.A))
        {
            player.TankData.TankDir = TankDir.Left;
        }
        else if (kState.IsKeyDown(Keys.D))
        {
            player.TankData.TankDir = TankDir.Right;
        }
        else if (kState.IsKeyDown(Keys.W))
        {
            player.TankData.TankDir = TankDir.Up;
        }
        else if (kState.IsKeyDown(Keys.S))
        {
            player.TankData.TankDir = TankDir.Down;
        }
        else
        {
            player.TankData.TankDir = (TankDir)(-1);
        }
        if (Global.LeftMouseClicked())
        {
            var shootPos = player.Position + shootDir * BARREL_LENGTH;
            entities.Add(GameEntities.MakeShell(shootPos, player.TankData.Barrel.Rotation, SHELL_SPEED));
        }

        TankController.UpdateTank(player, player.TankData, colliders);
        player.TankData.Barrel.Rotation = MathF.Atan2(shootDir.Y, shootDir.X);
    }

    private void updateEnemyStationary(Enemy enemy, float dt)
    {
        enemy.EnemyShootTime += dt;
        var shootDir = Vector2.Normalize(player.Position - enemy.Position);
        var shootAngle = MathF.Atan2(shootDir.Y, shootDir.X);
        enemy.TankData.Barrel.Rotation = MathF.Atan2(shootDir.Y, shootDir.X);

        if (enemy.EnemyShootTime > ENEMY_STATIONARY_SHOOT_DELTA)
        {
            entities.Add(GameEntities.MakeShell(enemy.Position + shootDir * BARREL_LENGTH, shootAngle, SHELL_SPEED));
            enemy.EnemyShootTime = 0f;
        }
    }

    private void updateShell(Shell shell, float dt)
    {
        shell.Position += new Vector2(MathF.Cos(shell.Direction), MathF.Sin(shell.Direction)) * shell.Speed * dt;
        shell.KillTime += dt;
        if (shell.KillTime > SHELL_KILL_TIME_MAX)
        {
            entities.Remove(shell);
        }
    }

    public override void Draw()
    {
        Renderer.Get().Draw( 
        (world) =>
        {
            Levels.DrawLayers(mapData, ["Ground", "Ponds", "Paths", "HouseWalls", "HouseDoors", "FencesBushes"]);

            foreach (var ent in entities.OrderBy(e => e.LayerDepth))
            {
                Renderer.DrawEntity(ent);
            }

            Levels.DrawLayers(mapData, ["HouseRoofs"]);
        }, 
        (screen) =>
        {
            
        });
    }

    public override void Unload()
    {
        foreach (var ent in entities)
        {
            ent.Texture.Dispose();
        }
        foreach (var tex in mapData.TilesetTextures.Values)
        {
            tex.Dispose();
        }
    }
}