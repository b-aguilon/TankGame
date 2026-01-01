using Engine;
using Engine.Collisions;

using System.Collections.Generic;

namespace Script;

public static class ShellController
{
    /*public static void UpdateShell(Shell shell, List<Entity> entities)
    {
        entities.sh
        var dt = Global.DELTA_TIME;
        shell.Position += new Vector2(MathF.Cos(shell.Direction), MathF.Sin(shell.Direction)) * shell.Speed * dt;
        shell.KillTime += dt;

        foreach (var ent in entities.Where
        (
            e => e is Tank || e is Shell && e != shell.ShotBy
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
                    hitBox = p.Collider;
                    if (RectCollider.ContainsPoint(hitBox, shell.Position))
                    {
                        removePlayer(p);
                    }
                    break;
                case Enemy e:
                    hitBox = e.Collider;
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
    }*/
}