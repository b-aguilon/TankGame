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
    private const float SHELL_KILL_TIME_MAX = 10f;

    private bool opennedURL = false;
    private bool playerDead = false;
    private bool won = false;

    private Vector2 playerPos;
    private MapData mapData;
    private Texture2D bitch;
    private Texture2D fuck;

    private readonly List<Entity> entities = new List<Entity>();
    private readonly List<RectCollider> colliders = new List<RectCollider>();

    public override void Load()
    {
        EnemyController.EnemyShot += OnEnemyShoot;
        bitch = Texture2D.FromFile(Global.Graphics.GraphicsDevice, "assets/png/bitch.png");
        fuck = Texture2D.FromFile(Global.Graphics.GraphicsDevice, "assets/png/fuck.png");

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
            switch (spawn.Name)
            {
                case "Player":
                    var player = GameEntities.MakePlayer(new(spawn.X, spawn.Y), speed:60);
                    addPlayer(player);
                    break;
                case "Moving":
                    enemy = GameEntities.MakeEnemyMoving(new(spawn.X, spawn.Y));
                    addEnemy(enemy);
                    break;
                case "Stationary":
                    enemy = GameEntities.MakeEnemyStationary(new(spawn.X, spawn.Y));
                    addEnemy(enemy);
                    break;
                case "Fast":
                    enemy = GameEntities.MakeEnemyFast(new(spawn.X, spawn.Y));
                    addEnemy(enemy);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnEnemyShoot(object sender, EventArgs e)
    {
        if (sender is Enemy enemy)
        {
            var shell = GameEntities.MakeShell(enemy.Position + enemy.TankData.Barrel.Direction * BARREL_LENGTH, enemy.TankData.Barrel.Rotation, enemy.ShellSpeed);
            shell.ShotBy = enemy;
            entities.Add(shell);
        }
    }

    public override void Update()
    {
        var dt = Global.DELTA_TIME;

        if (entities.Where(e => e is Enemy).Count() <= 0 && !opennedURL)
        {
            OpenUrl(Win.WINNER);
            opennedURL = true;
            won = true;
        }
        else if (entities.Where(e => e is Player).Count() <= 0 && !opennedURL)
        {
            OpenUrl(Win.LOSER);
            opennedURL = true;
            playerDead = true;
        }

        if (playerDead && Global.K_State.IsKeyDown(Keys.Enter))
        {
            changeState(new MainState());
        }

        foreach (var ent in entities.ToArray())
        {
            switch (ent)
            {
                case Player player:
                    updatePlayer(dt, player);
                    playerPos = player.Position;
                    break;
                case Shell shell:
                    updateShell(shell, dt);
                    break;
                case Enemy enemy:
                    EnemyController.UpdateEnemy(enemy, colliders, playerPos);
                    break;
                default:
                    break;
            }
        }

        Renderer.Get().CameraFollow(playerPos);
    }

    private void updatePlayer(float dt, Player player)
    {
        var kState = Global.K_State;
        var mouseWorldPos = Renderer.Get().GetWorldMousePos();
        player.TankData.Barrel.Direction = Vector2.Normalize(mouseWorldPos - player.TankData.Barrel.Position);

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
            var shootPos = player.Position + player.TankData.Barrel.Direction * BARREL_LENGTH;
            var shell = GameEntities.MakeShell(shootPos, player.TankData.Barrel.Rotation, player.ShellSpeed);
            shell.ShotBy = player;
            entities.Add(shell);
        }

        TankController.UpdateTank(player, player.TankData, colliders);
    }

    private void updateShell(Shell shell, float dt)
    {
        shell.Position += new Vector2(MathF.Cos(shell.Direction), MathF.Sin(shell.Direction)) * shell.Speed * dt;
        shell.KillTime += dt;

        foreach (var ent in entities.Where
        (
            e => e is Enemy || e is Player || e is Shell
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
                        entities.Remove(shell);
                        entities.Remove(s);
                    }
                    break;
                case Player p:
                    hitBox = p.TankData.Collider;
                    if (RectCollider.ContainsPoint(hitBox, shell.Position))
                    {
                        removePlayer(p);
                    }
                    break;
                case Enemy e:
                    hitBox = e.TankData.Collider;
                    if (shell.ShotBy is Player && RectCollider.ContainsPoint(hitBox, shell.Position))
                    {
                        removeEnemy(e);
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
                entities.Remove(shell);
            }
        }

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
            Levels.DrawLayers(mapData, ["Ground"]);

            foreach (var ent in entities.OrderBy(e => e.LayerDepth))
            {
                Renderer.DrawEntity(ent);
            }
        }, 
        (screen) =>
        {
            if (playerDead)
            {
                Global.Batch.Draw(bitch, Vector2.Zero, Color.White);
            }
            else if (won)
            {
                Global.Batch.Draw(fuck, Vector2.Zero, Color.White);
            }
        });
    }

    public override void Unload()
    {
        bitch.Dispose();

        foreach (var ent in entities)
        {
            ent.Texture.Dispose();
        }
        foreach (var tex in mapData.TilesetTextures.Values)
        {
            tex.Dispose();
        }
    }

    private void addPlayer(Player p)
    {
        entities.Add(p);
        entities.Add(p.TankData.Barrel);
    }

    private void removePlayer(Player p)
    {
        entities.Remove(p);
        entities.Remove(p.TankData.Barrel);
    }

    private void addEnemy(Enemy e)
    {
        entities.Add(e);
        entities.Add(e.TankData.Barrel);
    }

    private void removeEnemy(Enemy e)
    {
        entities.Remove(e);
        entities.Remove(e.TankData.Barrel);
    }


    //remove this bullshit
    private void OpenUrl(string url)
    {
        try
        {
            System.Diagnostics.Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                System.Diagnostics.Process.Start("xdg-open", url);
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
            {
                System.Diagnostics.Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
}