using Engine;
using Engine.Collisions;

namespace Script;

public class GameEntities
{
    private const int TANK_DIMENSIONS = 17;

    public static Player MakePlayer(Vector2 pos, float speed, int cannonHeadRotationPoint=3)
    {
        var player = new Player();
        player.Texture = Texture2D.FromFile(Global.Graphics.GraphicsDevice, $"{Global.ASSETS_PATH}png/tankPlayer.png");
        player.Source = new Rectangle(0, 0, player.Texture.Width, player.Texture.Height);
        player.Width = TANK_DIMENSIONS;
        player.Height = TANK_DIMENSIONS;
        player.Position = pos;
        player.RotationPivot = new(player.Texture.Width / 2f, player.Texture.Height / 2f);
        player.DrawOffset = player.RotationPivot;
        player.TankData = new();
        player.TankData.Collider = new RectCollider(player.Position - player.DrawOffset / 2, new(player.Width, player.Height));
        player.TankData.Direction = Vector2.UnitX;
        player.TankData.TankDir = (TankDir)(-1);
        player.TankData.Barrel = MakeTankBarrel(player.Position, cannonHeadRotationPoint);
        player.TankData.Speed = speed;

        return player;
    }

    public static Enemy MakeTankStationary(Vector2 pos, int cannonHeadRotationPoint=3)
    {
        var enemy = new Enemy();
        enemy.Texture = Texture2D.FromFile(Global.Graphics.GraphicsDevice, $"{Global.ASSETS_PATH}png/tankPlayer.png");
        enemy.Source = new Rectangle(0, 0, enemy.Texture.Width, enemy.Texture.Height);
        enemy.Width = TANK_DIMENSIONS;
        enemy.Height = TANK_DIMENSIONS;
        enemy.Position = pos;
        enemy.RotationPivot = new(enemy.Texture.Width / 2f, enemy.Texture.Height / 2f);
        enemy.DrawOffset = enemy.RotationPivot;
        enemy.TankData = new();
        enemy.TankData.Collider = new RectCollider(enemy.Position - enemy.DrawOffset / 2, new(enemy.Width, enemy.Height));
        enemy.TankData.Direction = Vector2.UnitX;
        enemy.TankData.TankDir = (TankDir)(-1);
        enemy.TankData.Barrel = MakeTankBarrel(enemy.Position, cannonHeadRotationPoint);
        enemy.TankData.Speed = 0;
        
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

    public static Barrel MakeTankBarrel(Vector2 pos, int cannonHeadRotationPoint)
    {
        var barrel = new Barrel();
        barrel.Texture = Texture2D.FromFile(Global.Graphics.GraphicsDevice, $"{Global.ASSETS_PATH}png/tankBarrel.png");
        barrel.Source = new Rectangle(0, 0, barrel.Texture.Width, barrel.Texture.Height);
        barrel.Width = barrel.Texture.Width;
        barrel.Height = barrel.Texture.Height;
        barrel.Position = pos;
        barrel.RotationPivot = new(cannonHeadRotationPoint, barrel.Height / 2);
        barrel.DrawOffset = barrel.RotationPivot;

        return barrel;
    }
}

public class Player : Entity
{
    public TankData TankData;
}

public class Enemy : Entity
{
    public TankData TankData;
    public float EnemyShootTime = 0f;
}

public class Shell : Entity
{
    public float Speed;
    public float Direction;
    public float KillTime = 0f;
}

public class TankData
{
    public RectCollider Collider;
    public Barrel Barrel;
    public Vector2 Direction;
    public TankDir TankDir;
    public float Speed;
}

public class Barrel : Entity {}

public enum TankDir 
{UpLeft, Up, UpRight, Right, DownRight, Down, DownLeft, Left}