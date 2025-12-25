namespace Engine.Shapes;

public static class Collision
{
    public static void ResolveCollision(ref Vector2 position, ref Rectangle rect, Rectangle other)
    {
        rect.X = (int)position.X;
        rect.Y = (int)position.Y;

        bool collidingLeftOrRight = 
        CollidingLeft(rect, other) || CollidingRight(rect, other);
        bool collidingTopOrBottom = 
        CollidingTop(rect, other) || CollidingBottom(rect, other);

        if (collidingLeftOrRight && collidingTopOrBottom)
        {
            const int DELTA = 1;
            if (CollidingLeft(rect, other) && CollidingTop(rect, other))
            {
                position.Y -= DELTA;
                position.X -= DELTA;
            }
            else if (CollidingLeft(rect, other) && CollidingBottom(rect, other))
            {
                position.Y += DELTA;
                position.X -= DELTA;
            }
            else if (CollidingRight(rect, other) && CollidingTop(rect, other))
            {
                position.Y -= DELTA;
                position.X += DELTA;
            }
            else if (CollidingRight(rect, other) && CollidingBottom(rect, other))
            {
                position.Y += DELTA;
                position.X += DELTA;
            }
        }
        else
        {
            if (CollidingTop(rect, other))
            {
                position.Y = other.Top - rect.Height;
            }
            else if (CollidingBottom(rect, other))
            {
                position.Y = other.Bottom;
            }
            else if (CollidingLeft(rect, other))
            {
                position.X = other.Left - rect.Width;
            }
            else if (CollidingRight(rect, other))
            {
                position.X = other.Right;
            }
        }
    }

    public static bool CollidingTop(Rectangle rect, Rectangle other, float nextMove=0)
    {
        return rect.Y + rect.Height + nextMove > other.Y &&
               rect.X < other.X + other.Width && rect.X + rect.Width > other.X &&
               rect.Y < other.Y;
    }

    public static bool CollidingBottom(Rectangle rect, Rectangle other, float nextMove=0)
    {
        return rect.Y + nextMove < other.Y + other.Height &&
               rect.X < other.X + other.Width && rect.X + rect.Width > other.X &&
               rect.Y + rect.Height > other.Y + other.Height;
    }

    public static bool CollidingLeft(Rectangle rect, Rectangle other, float nextMove=0)
    {
        return rect.X + rect.Width + nextMove > other.X &&
               rect.Y < other.Y + other.Height && rect.Y + rect.Height > other.Y &&
               rect.X < other.X;
    }

    public static bool CollidingRight(Rectangle rect, Rectangle other, float nextMove=0)
    {
        return rect.X + nextMove < other.X + other.Width &&
               rect.Y < other.Y + other.Height && rect.Y + rect.Height > other.Y &&
               rect.X + rect.Width > other.X + other.Width;
    }
}

public enum RectCollideSide {None, Top, Bottom, Left, Right}

public struct Circle
{
    public int X {get;set;}
    public int Y {get;set;}
    public int Radius {get;set;}

    public Circle()
    {
        X = 0;
        Y = 0;
        Radius = 0;
        throw new NotImplementedException();
    }

    public Circle(int x, int y, int radius)
    {
        X = x;
        Y = y;
        Radius = radius;
        throw new NotImplementedException();
    }

    public Vector2 GetCircleEdge(float angleRadian)
    {
        throw new NotImplementedException();
    }
}