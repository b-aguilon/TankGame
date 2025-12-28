using System.Collections.Generic;

namespace Engine.Collisions;

public static class Collision
{
    public static bool RayVsRect(Vector2 origin, Vector2 dir, RectCollider rect, 
        ref Vector2 contactPoint, ref Vector2 contactNormal, ref float hitNear)
    {
        var near = (rect.Position - origin) / dir;
        var far = (rect.Position + rect.Size - origin) / dir;
        if (float.IsNaN(near.X) || float.IsNaN(near.Y))
        {
            return false;
        }
        if (float.IsNaN(far.X) || float.IsNaN(far.Y))
        {
            return false;
        }

        if (near.X > far.X)
        {
            var nearX = near.X;
            near.X = far.X;
            far.X = nearX;
        }
        if (near.Y > far.Y)
        {
            var nearY = near.Y;
            near.Y = far.Y;
            far.Y = nearY;
        }
        
        if (near.X > far.Y || near.Y > far.X)
        {
            return false;
        }

        hitNear = MathF.Max(near.X, near.Y);
        if (hitNear > 1f || hitNear < 0f)
        {
            return false;
        }

        contactPoint = origin + hitNear * dir;

        if (near.X > near.Y)
        {
            if (dir.X < 0)
                contactNormal = new(1, 0);
            else
                contactNormal = new(-1, 0);
        }
        else if (near.X < near.Y)
        {
            if (dir.Y < 0)
                contactNormal = new(0, 1);
            else
                contactNormal = new(0, -1);
        }

        return true;
    }

    public static bool DynamicRectVsRect(RectCollider rect, RectCollider target, 
        ref Vector2 contactPoint, ref Vector2 contactNormal, ref float distance)
    {
        var dt = Global.DELTA_TIME;
        if (rect.Velocity.X == 0 && rect.Velocity.Y == 0)
            return false;

        RectCollider expandedTarget = new();
        expandedTarget.Position = target.Position - rect.Size / 2f;
        expandedTarget.Size = target.Size + rect.Size;

        if (RayVsRect(RectCollider.GetCenter(rect), rect.Velocity*dt, expandedTarget, 
            ref contactPoint, ref contactNormal, ref distance))
        {
            return true;
        }

        return false;
    }

    public static void HandleRectVsRectCollisions(RectCollider collider, List<RectCollider> colliders)
    {
        float distance = 0f;
        Vector2 contact = new(), normal = new();

        var sorted = new List<(float dist, RectCollider col, Vector2 norm)>();

        foreach (var c in colliders)
        {
            if (DynamicRectVsRect(collider, c, ref contact, ref normal, ref distance) && c != collider)
            {
                sorted.Add((distance, c, normal));
            }
        }

        foreach (var tuple in sorted.OrderBy(t => t.dist))
        {
            var vel = collider.Velocity;
            collider.Velocity += (tuple.norm * new Vector2(MathF.Abs(vel.X), MathF.Abs(vel.Y)) * (1f - tuple.dist));
        }
    }
}

public class RectCollider
{
    public Vector2 Position;
    public Vector2 Size;
    public Vector2 Velocity;

    public RectCollider()
    {
        Position = Vector2.Zero;
        Size = Vector2.Zero;
        Velocity = Vector2.Zero;
    }

    public RectCollider(Vector2 pos, Vector2 size)
    {
        Position = pos;
        Size = size;
        Velocity = Vector2.Zero;
    }

    public static Vector2 GetCenter(RectCollider rect)
    {
        return rect.Position + (rect.Size / 2f);
    }

    public static bool ContainsPoint(RectCollider rect, Vector2 point)
    {
        return point.X >= rect.Position.X && point.X <= rect.Position.X + rect.Size.X
            && point.Y >= rect.Position.Y && point.Y <= rect.Position.Y + rect.Size.Y;
    }

    public static bool Intersects(RectCollider rect, RectCollider other)
    {
        return
        rect.Position.X < other.Position.X + other.Size.X &&
        rect.Position.Y < other.Position.Y + other.Size.Y &&
        rect.Position.X + rect.Size.X > other.Position.X &&
        rect.Position.Y + rect.Size.Y > other.Position.Y;
    }
}

public class CircleCollider
{
    public Vector2 Position;
    public float Radius;
    public Vector2 Velocity;

    public CircleCollider()
    {
        Position = Vector2.Zero;
        Radius = 0;
        Velocity = Vector2.Zero;
        throw new NotImplementedException();
    }

    public CircleCollider(Vector2 pos, float radius)
    {
        Position = pos;
        Radius = radius;
        Velocity = Vector2.Zero;
        throw new NotImplementedException();
    }

    public static Vector2 GetCircleEdge(float angleRadian)
    {
        throw new NotImplementedException();
    }
}