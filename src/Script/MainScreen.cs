using Engine;
using Engine.Graphics;
using Engine.Tilemap;
using Engine.Collisions;

using DotTiled;
using System.Collections.Generic;

namespace Script;

class MainScreen : GameScreen
{
    private const int BARREL_LENGTH = 15;
    private const float SHELL_KILL_TIME_MAX = 10f;

    private Player player;
    private MapData mapData;

    private readonly List<RectCollider> colliders = new List<RectCollider>();

    protected override void loadGameScreen()
    {
        GameEntities.AddShootListener(onEntityShoot);

        var map = Levels.Loader.LoadMap("assets/maps/levels/1.tmx");
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
                case "Player":
                    if (playerSpawned)
                    {
                        throw new Exception("Muliple player spawns");
                    }
                    this.player = GameEntities.MakePlayer(new(spawn.X, spawn.Y), speed:60);
                    Entities.TriggerAddEntity(player);
                    playerSpawned = true;
                    break;
                case "Moving":
                    enemy = GameEntities.MakeEnemyMoving(new(spawn.X, spawn.Y));
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
        Shell shell;
        var entity = (Entity)sender;
        if (sender is not Entity)
            throw new Exception("Only entities can shoot");

        if (sender is Tank tank)
        {
            shell = GameEntities.MakeShell(entity.Position + tank.Barrel.Direction * BARREL_LENGTH, tank.Barrel.Rotation, tank.ShellSpeed);
            shell.ShotBy = entity;
            Entities.TriggerAddEntity(shell);
        }
    }

    protected override void entityAddedEffects(Entity entity)
    {
        if (entity is Tank tank)
        {
            Entities.TriggerAddEntity(tank.Barrel);
        }
    }

    protected override void entityRemovedEffects(Entity entity)
    {
        if (entity is Tank tank)
        {
            Entities.TriggerRemoveEntity(tank.Barrel);
        }
    }

    public override void Update()
    {
        var dt = Global.DELTA_TIME;

        foreach (var ent in Entities.GetEntities().ToArray())
        {
            switch (ent)
            {
                case Player player:
                    PlayerController.UpdatePlayer(player);
                    break;
                case Shell shell:
                    updateShell(shell, dt);
                    break;
                case Enemy enemy:
                    EnemyController.UpdateEnemy(enemy, player.Position);
                    break;
                default:
                    break;
            }

            if (ent is Tank tank)
            {
                TankController.UpdateTank(ent, tank, colliders);
            }
        }

        Renderer.Get().CameraFollow(player.Position);
    }

    private void updateShell(Shell shell, float dt)
    {
        shell.Position += new Vector2(MathF.Cos(shell.Direction), MathF.Sin(shell.Direction)) * shell.Speed * dt;
        shell.KillTime += dt;

        foreach (var ent in Entities.GetEntities().Where
        (
            e => e is Enemy || e is Player || e is Shell && e != shell.ShotBy
        )
        .ToArray())
        {
            var hitBox = new RectCollider();
            switch (ent)
            {
                case Shell s:
                    hitBox = new RectCollider(new(s.Position.X, s.Position.Y), new(s.Width, s.Height));
                    if (s != shell && RectCollider.ContainsPoint(hitBox, shell.Position))
                    {
                        Entities.TriggerRemoveEntity(shell);
                        Entities.TriggerRemoveEntity(s);
                    }
                    break;
                case Player p:
                    hitBox = p.Collider;
                    if (RectCollider.ContainsPoint(hitBox, shell.Position))
                    {
                        Entities.TriggerRemoveEntity(p);
                    }
                    break;
                case Enemy e:
                    hitBox = e.Collider;
                    if (shell.ShotBy is Player && RectCollider.ContainsPoint(hitBox, shell.Position))
                    {
                        Entities.TriggerRemoveEntity(e);
                    }
                    break;
                default:
                    break;
            }
        }

        foreach (var collider in colliders)
        {
            if (RectCollider.ContainsPoint(collider, shell.Position))
            {
                Entities.TriggerRemoveEntity(shell);
            }
        }

        if (shell.KillTime > SHELL_KILL_TIME_MAX)
        {
            Entities.TriggerRemoveEntity(shell);
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