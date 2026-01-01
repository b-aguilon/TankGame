using Engine;

namespace Script;

public static class EnemyController
{
    public static void UpdateEnemy(Enemy enemy, Vector2 target)
    {
        moveTowardTarget(enemy, target);
        updateEnemyShoot(enemy, target);
    }

    private static void moveTowardTarget(Enemy enemy, Vector2 target)
    {
        if (Vector2.Distance(enemy.Position, target) < enemy.MinFollowDistance)
        {
        }
        else if (enemy.Position.X > target.X && enemy.Position.Y > target.Y)
        {
            enemy.TankDir = TankDir.UpLeft;
        }
        else if(enemy.Position.X > target.X && enemy.Position.Y < target.Y)
        {
            enemy.TankDir = TankDir.DownLeft;
        }
        else if(enemy.Position.X < target.X && enemy.Position.Y < target.Y)
        {
            enemy.TankDir = TankDir.DownRight;
        }
        else if(enemy.Position.X < target.X && enemy.Position.Y > target.Y)
        {
            enemy.TankDir = TankDir.UpRight;
        }
    }

    private static void updateEnemyShoot(Enemy enemy, Vector2 target)
    {
        var dt = Global.DELTA_TIME;
        enemy.EnemyShootTime += dt;
        enemy.Barrel.Direction = Vector2.Normalize(target - enemy.Position);

        if (enemy.EnemyShootTime > enemy.ShootDelta)
        {
            GameEntities.TriggerEntityOnShoot(enemy);
            enemy.EnemyShootTime = 0f;
        }
    }
}