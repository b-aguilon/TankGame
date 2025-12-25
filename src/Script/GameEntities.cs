using Engine;

namespace Script;

public class GameEntities
{
    public static Player MakePlayer(Vector2 pos, float speed, int cannonHeadRotationPoint=3)
    {
        var player = new Player();
        player.Texture = Texture2D.FromFile(Global.Graphics.GraphicsDevice, $"{Global.ASSETS_PATH}png/tankPlayer.png");
        player.Source = new Rectangle(0, 0, player.Texture.Width, player.Texture.Height);
        player.Width = player.Texture.Width;
        player.Height = player.Texture.Height;
        player.Position = pos;
        player.Origin = new(player.Width / 2f, player.Height / 2f);
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
        enemy.Width = enemy.Texture.Width;
        enemy.Height = enemy.Texture.Height;
        enemy.Position = pos;
        enemy.Origin = new(enemy.Width / 2f, enemy.Height / 2f);
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
        barrel.Origin = new(cannonHeadRotationPoint, barrel.Height / 2);

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

public struct TankData
{
    public Barrel Barrel;
    public Vector2 Direction;
    public TankDir TankDir;
    public float Speed;
}

public class Barrel : Entity {}

public enum TankDir 
{UpLeft, Up, UpRight, Right, DownRight, Down, DownLeft, Left}