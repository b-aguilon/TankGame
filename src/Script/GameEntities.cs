using Engine;
using Engine.Collisions;

namespace Script;

public class GameEntities
{
    private const int TANK_DIMENSIONS = 17;
    private const int BARREL_ROT_POINT = 3;

    private const int PLAYER_SPEED = 180;
    private const int PLAYER_SHELL_SPEED = 240;

    private const int ENEMY_NORMAL_SPEED = 45;
    private const int ENEMY_FAST_SPEED = 180;

    private const int ENEMY_STATIONARY_SHELL_SPEED = 280;
    private const int ENEMY_NORMAL_SHELL_SPEED = 110;
    private const int ENEMY_FAST_SHELL_SPEED = 240;

    private const int ENEMY_MIN_FOLLOW_DISTANCE = 65;

    public static Player MakePlayer(Vector2 pos)
    {
        var player = new Player();
        player.Texture = Texture2D.FromFile(Global.Graphics.GraphicsDevice, $"{Global.ASSETS_PATH}png/tankPlayer.png");
        player.Source = new Rectangle(0, 0, player.Texture.Width, player.Texture.Height);
        player.Width = TANK_DIMENSIONS;
        player.Height = TANK_DIMENSIONS;
        player.LayerDepth = 0.4f;
        player.Position = pos;
        player.RotationPivot = new(player.Texture.Width / 2f, player.Texture.Height / 2f);
        player.DrawOffset = player.RotationPivot;
        player.ShellSpeed = PLAYER_SHELL_SPEED;
        player.Collider = new RectCollider(player.Position - player.DrawOffset / 2, new(player.Width, player.Height));
        player.Direction = Vector2.UnitX;
        player.TankDir = (TankDir)(-1);
        player.Barrel = MakeTankBarrel(player.Position, BARREL_ROT_POINT);
        player.Barrel.LayerDepth = 0.45f;
        player.Speed = PLAYER_SPEED;

        return player;
    }

    public static Enemy MakeEnemyStationary(Vector2 pos)
    {
        return MakeEnemy
        (
            pos, 
            speed:0, 
            minFollowDistance:ENEMY_MIN_FOLLOW_DISTANCE, 
            shellSpeed:ENEMY_STATIONARY_SHELL_SPEED, 
            "tank.png", 
            "tankBarrelStationary.png"
        );
    }

    public static Enemy MakeEnemyMoving(Vector2 pos)
    {
        return MakeEnemy
        (
            pos, 
            speed:ENEMY_NORMAL_SPEED, 
            minFollowDistance:ENEMY_MIN_FOLLOW_DISTANCE, 
            shellSpeed:ENEMY_NORMAL_SHELL_SPEED, 
            "stationaryTank.png", 
            "stationaryBarrel.png"
        );
    }

    public static Enemy MakeEnemyFast(Vector2 pos)
    {
        return MakeEnemy
        (
            pos, 
            speed:ENEMY_FAST_SPEED, 
            minFollowDistance:ENEMY_MIN_FOLLOW_DISTANCE, 
            shellSpeed:ENEMY_FAST_SHELL_SPEED, 
            "tankFast.png", 
            "tankFastBarrel.png"
        );
    }

    private static Enemy MakeEnemy(Vector2 pos, float speed, int minFollowDistance, float shellSpeed, string textureFile, string barrelFile)
    {
        var enemy = new Enemy();
        enemy.Texture = Texture2D.FromFile(Global.Graphics.GraphicsDevice, $"{Global.ASSETS_PATH}png/{textureFile}");
        enemy.Source = new Rectangle(0, 0, enemy.Texture.Width, enemy.Texture.Height);
        enemy.Width = TANK_DIMENSIONS;
        enemy.Height = TANK_DIMENSIONS;
        enemy.Position = pos;
        enemy.RotationPivot = new(enemy.Texture.Width / 2f, enemy.Texture.Height / 2f);
        enemy.DrawOffset = enemy.RotationPivot;
        enemy.ShellSpeed = shellSpeed;
        enemy.Collider = new RectCollider(enemy.Position - enemy.DrawOffset / 2, new(enemy.Width, enemy.Height));
        enemy.Direction = Vector2.UnitX;
        enemy.TankDir = TankDir.Right;
        enemy.Barrel = MakeTankBarrel(enemy.Position, BARREL_ROT_POINT);
        enemy.Barrel.Texture = Texture2D.FromFile(Global.Graphics.GraphicsDevice, $"{Global.ASSETS_PATH}png/{barrelFile}");
        enemy.Barrel.LayerDepth = 0.1f;
        enemy.MinFollowDistance = minFollowDistance;
        enemy.Speed = speed;
        
        return enemy;
    }

    public static Shell MakeShell(Vector2 pos, float direction, float speed)
    {
        var shell = new Shell();
        shell.Texture = Texture2D.FromFile(Global.Graphics.GraphicsDevice, $"{Global.ASSETS_PATH}png/shell.png");
        shell.Source = new Rectangle(0, 0, shell.Texture.Width, shell.Texture.Height);
        shell.Width = shell.Texture.Width;
        shell.Height = shell.Texture.Height;
        shell.Position = pos;
        shell.Direction = direction;
        shell.Speed = speed;

        return shell;
    }

    private static Barrel MakeTankBarrel(Vector2 pos, int cannonHeadRotationPoint)
    {
        var barrel = new Barrel();
        barrel.Texture = Texture2D.FromFile(Global.Graphics.GraphicsDevice, $"{Global.ASSETS_PATH}png/tankBarrel.png");;
        barrel.Source = new Rectangle(0, 0, barrel.Texture.Width, barrel.Texture.Height);
        barrel.Width = barrel.Texture.Width;
        barrel.Height = barrel.Texture.Height;
        barrel.Position = pos;
        barrel.RotationPivot = new(cannonHeadRotationPoint, barrel.Height / 2);
        barrel.DrawOffset = barrel.RotationPivot;

        return barrel;
    }

    private static event EventHandler tankShot;

    public static void AddShootListener(EventHandler handler)
    {
        tankShot += handler;
    }

    public static void RemoveShootListener(EventHandler handler)
    {
        tankShot -= handler;
    }

    public static void ClearShootListeners()
    {
        tankShot = null;
    }

    public static void TriggerEntityOnShoot(Entity ent)
    {
        tankShot?.Invoke(ent, EventArgs.Empty);
    }
}

public class Player : Entity, TankData
{
    public RectCollider Collider {get; set;}
    public Barrel Barrel {get; set;}
    public Vector2 Direction {get; set;}
    public TankDir TankDir {get; set;}
    public float Speed {get; set;}
    public float ShellSpeed {get; set;} = 70;
}

public class Enemy : Entity, TankData
{
    public float EnemyShootTime = 0f;
    public int MinFollowDistance = 65;
    public float ShootDelta = 1f;

    public RectCollider Collider {get; set;}
    public Barrel Barrel {get; set;}
    public Vector2 Direction {get; set;}
    public TankDir TankDir {get; set;}
    public float Speed {get; set;}
    public float ShellSpeed {get; set;} = 70;
}

public class Shell : Entity
{
    public Entity ShotBy;
    public float Speed;
    public float Direction;
    public float KillTime = 0f;
}
public class Barrel : Entity
{
    public Vector2 Direction;
}

public enum TankDir 
{UpLeft, Up, UpRight, Right, DownRight, Down, DownLeft, Left}

public interface TankData
{
    public RectCollider Collider {get; set;}
    public Barrel Barrel {get; set;}
    public Vector2 Direction {get; set;}
    public TankDir TankDir {get; set;}
    public float Speed {get; set;}
    public float ShellSpeed {get; set;}
}