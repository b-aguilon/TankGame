using System.Collections.Generic;
using Engine;
using Engine.Collisions;

namespace Script;

public static class EnemyController
{
    public static event EventHandler EnemyShot;

    public static void UpdateEnemy(Enemy enemy, List<RectCollider> colliders, Vector2 target)
    {
        moveTowardTarget(enemy, colliders, target);
        updateEnemyShoot(enemy, target);
    }

    private static void moveTowardTarget(Enemy enemy, List<RectCollider> colliders, Vector2 target)
    {
        if (Vector2.Distance(enemy.Position, target) < enemy.MinFollowDistance)
        {
        }
        else if (enemy.Position.X > target.X && enemy.Position.Y > target.Y)
        {
            enemy.TankData.TankDir = TankDir.UpLeft;
        }
        else if(enemy.Position.X > target.X && enemy.Position.Y < target.Y)
        {
            enemy.TankData.TankDir = TankDir.DownLeft;
        }
        else if(enemy.Position.X < target.X && enemy.Position.Y < target.Y)
        {
            enemy.TankData.TankDir = TankDir.DownRight;
        }
        else if(enemy.Position.X < target.X && enemy.Position.Y > target.Y)
        {
            enemy.TankData.TankDir = TankDir.UpRight;
        }

        TankController.UpdateTank(enemy, enemy.TankData, colliders);
    }

    private static void updateEnemyShoot(Enemy enemy, Vector2 target)
    {
        var dt = Global.DELTA_TIME;
        enemy.EnemyShootTime += dt;
        enemy.TankData.Barrel.Direction = Vector2.Normalize(target - enemy.Position);

        if (enemy.EnemyShootTime > enemy.ShootDelta)
        {
            EnemyShot.Invoke(enemy, EventArgs.Empty);
            enemy.EnemyShootTime = 0f;
        }
    }
}