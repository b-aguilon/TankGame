using Engine;
using Engine.Collisions;

namespace Script;

public class GameEntities
{
    public const float ENEMY_SHOOT_VOLUME = 0.2f;
    public const float PLAYER_SHOOT_VOLUME = 1f;

    private const int TANK_DIMENSIONS = 17;
    private const int BARREL_ROT_POINT = 3;

    private const int PLAYER_SPEED = 75;
    private const int PLAYER_SHELL_SPEED = 150;
    private const float PLAYER_FIRERATE = 0.3f;
    private const float PLAYER_ROT_SPEED = MathHelper.Pi / 70f;

    private const int ENEMY_NORMAL_SPEED = 50;
    private const int ENEMY_NORMAL_SHELL_SPEED = 120;

    private const int ENEMY_FAST_SPEED = 180;
    private const int ENEMY_FAST_SHELL_SPEED = 240;

    private const int ENEMY_STATIONARY_SHELL_SPEED = 300;

    private const int ENEMY_MIN_FOLLOW_DISTANCE = 65;

    private const int ENEMY_FOLLOW_DISTANCE_RANGE = 40;
    private const int ENEMY_SPEED_RANGE = 50;
    private const float ENEMY_MAX_FIRERATE = 1.6f;
    private const float ENEMY_MIN_FIRERATE = 0.6f;
    private const float ENEMY_MAX_ROT_SPEED = MathHelper.Pi / 50f;
    private const float ENEMY_MIN_ROT_SPEED = MathHelper.Pi / 110f;

    public static Player MakePlayer(Vector2 pos)
    {
        var player = new Player();
        player.Texture = Global.GetTexture("tankPlayer.png");
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
        player.Barrel = makeTankBarrel(player.Position, BARREL_ROT_POINT, Global.GetTexture("tankBarrel.png"));
        player.Barrel.LayerDepth = 0.45f;
        player.Speed = PLAYER_SPEED;
        player.RotationSpeed = PLAYER_ROT_SPEED;
        player.ShootDelta = PLAYER_FIRERATE;

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
            "tankBarrelStationary.png",
            EnemyType.Stationary
        );
    }

    public static Enemy MakeEnemyNormal(Vector2 pos)
    {
        return MakeEnemy
        (
            pos, 
            speed:ENEMY_NORMAL_SPEED, 
            minFollowDistance:ENEMY_MIN_FOLLOW_DISTANCE, 
            shellSpeed:ENEMY_NORMAL_SHELL_SPEED, 
            "stationaryTank.png", 
            "stationaryBarrel.png",
            EnemyType.Normal
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
            "tankFastBarrel.png",
            EnemyType.Fast
        );
    }

    private static Enemy MakeEnemy
    (
        Vector2 pos, 
        int speed, 
        int minFollowDistance, 
        int shellSpeed, 
        string textureFile, 
        string barrelFile, 
        EnemyType type
    )
    {
        var random = Global.Rng;

        var enemy = new Enemy();
        enemy.Texture = Global.GetTexture(textureFile);
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
        var barrelTexture = Global.GetTexture(barrelFile);
        enemy.Barrel = makeTankBarrel(enemy.Position, BARREL_ROT_POINT, barrelTexture);
        enemy.Barrel.LayerDepth = 0.1f;
        enemy.MinFollowDistance = minFollowDistance;
        enemy.EnemyType = type;

        if (speed != 0)
            enemy.Speed = speed + random.Next(-ENEMY_SPEED_RANGE / 2, ENEMY_SPEED_RANGE / 2) + 1;
        else
            enemy.Speed = 0;
        enemy.RotationSpeed = (float)random.NextDouble() * (ENEMY_MAX_ROT_SPEED - ENEMY_MIN_ROT_SPEED) + ENEMY_MIN_ROT_SPEED;
        enemy.ShootDelta = (float)random.NextDouble() * (ENEMY_MAX_FIRERATE - ENEMY_MIN_FIRERATE) + ENEMY_MIN_FIRERATE;
        enemy.MinFollowDistance = ENEMY_MIN_FOLLOW_DISTANCE + random.Next(-ENEMY_FOLLOW_DISTANCE_RANGE / 2, ENEMY_FOLLOW_DISTANCE_RANGE / 2) + 1;
        
        return enemy;
    }

    public static Shell MakeShell(Vector2 pos, string texture, float direction, float speed)
    {
        var shell = new Shell();
        shell.Texture = Global.GetTexture(texture);
        shell.Source = new Rectangle(0, 0, shell.Texture.Width, shell.Texture.Height);
        shell.Width = shell.Texture.Width;
        shell.Height = shell.Texture.Height;
        shell.Position = pos;
        shell.Direction = direction;
        shell.Speed = speed;

        return shell;
    }

    private static Barrel makeTankBarrel(Vector2 pos, int cannonHeadRotationPoint, Texture2D barrelFile)
    {
        var barrel = new Barrel();
        barrel.Texture = barrelFile;
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
    public float ShootDelta = 0.1f;
    public float ShootTime = 0f;

    public RectCollider Collider {get; set;}
    public Barrel Barrel {get; set;}
    public Vector2 Direction {get; set;}
    public TankDir TankDir {get; set;}
    public float Speed {get; set;}
    public float ShellSpeed {get; set;} = 70;
    public float RotationSpeed {get; set;}
}

public class Enemy : Entity, TankData
{
    public EnemyType EnemyType;
    public float EnemyShootTime = 0f;
    public int MinFollowDistance = 65;
    public float ShootDelta = 1f;

    public RectCollider Collider {get; set;}
    public Barrel Barrel {get; set;}
    public Vector2 Direction {get; set;}
    public TankDir TankDir {get; set;}
    public float Speed {get; set;}
    public float ShellSpeed {get; set;} = 70;
    public float RotationSpeed {get; set;}
}

public enum EnemyType { Stationary, Normal, Fast }

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
    public float RotationSpeed {get; set;}
}