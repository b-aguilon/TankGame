using Engine;
using Engine.Collisions;

using System.Collections.Generic;

namespace Script;

public static class ShellController
{
    private const float KILL_TIME_MAX = 15f;

    public static void UpdateShell(Shell shell, IEnumerable<RectCollider> colliders)
    {
        var dt = Global.DELTA_TIME;
        shell.Position += new Vector2(MathF.Cos(shell.Direction), MathF.Sin(shell.Direction)) * shell.Speed * dt;
        shell.KillTime += dt;
        if (shell.KillTime > KILL_TIME_MAX)
        {
            Entities.TriggerRemoveEntity(shell);
        }

        foreach (var entity in Entities.GetEntities().Where
        (
            e => e is TankData || e is Shell && e != shell.ShotBy
        )
        .ToArray())
        {
            collideWithEntities(shell, entity);
        }

        collideWithColliders(shell, colliders);
    }

    private static void collideWithEntities(Shell shell, Entity entity)
    {
        RectCollider otherHitbox;
        var hitBox = new RectCollider
        (
            new(shell.Position.X, shell.Position.Y), 
            new(shell.Width, shell.Height)
        );
        switch (entity)
        {
            case Shell otherShell:
                otherHitbox = new RectCollider
                (
                    new(otherShell.Position.X, otherShell.Position.Y), 
                    new(otherShell.Width, otherShell.Height)
                );
                if (RectCollider.Intersects(hitBox, otherHitbox) && otherShell != shell)
                {
                    Entities.TriggerRemoveEntity(shell);
                    Entities.TriggerRemoveEntity(otherShell);
                }
                break;
            case Player player:
                otherHitbox = player.Collider;
                if (RectCollider.Intersects(hitBox, otherHitbox))
                {
                    Entities.TriggerRemoveEntity(player);
                }
                break;
            case Enemy enemy:
                otherHitbox = enemy.Collider;
                var collidersIntersect = RectCollider.Intersects(hitBox, otherHitbox);
                if (!collidersIntersect)
                {
                    return;
                }
                if (shell.ShotBy is Player)
                {
                    Entities.TriggerRemoveEntity(enemy);
                }
                else if (shell.ShotBy is Enemy)
                {
                    Entities.TriggerRemoveEntity(shell);
                }
                break;
            default:
                break;
        }
    }

    private static void collideWithColliders(Shell shell, IEnumerable<RectCollider> colliders)
    {
        foreach (var collider in colliders)
        {
            if (RectCollider.ContainsPoint(collider, shell.Position))
            {
                Entities.TriggerRemoveEntity(shell);
            }
        }
    }
}