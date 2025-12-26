using System.Collections.Generic;
using Engine;
using Engine.Shapes;

namespace Script;

public static class TankController
{
    private const float TANK_ROT_SPEED = MathHelper.Pi / 70f;
    private const float EPSILON = MathHelper.Pi / 30f;

    public static void MoveTank(Entity ent, List<Entity> entities, ref TankData tankData)
    {
        const float PI = MathHelper.Pi;
        bool moving = true;

        switch (tankData.TankDir)
        {
            case TankDir.Left:
                changeTankRotation(ent, PI);
                break;
            case TankDir.Right:
                changeTankRotation(ent, 0f);
                break;
            case TankDir.Up:
                changeTankRotation(ent, 3*PI/2f);
                break;
            case TankDir.Down:
                changeTankRotation(ent, PI/2f);
                break;
            case TankDir.UpLeft:
                changeTankRotation(ent, 5*PI/4f);
                break;
            case TankDir.UpRight:
                changeTankRotation(ent, 7*PI/4f);
                break;
            case TankDir.DownLeft:
                changeTankRotation(ent, 3*PI/4f);
                break;
            case TankDir.DownRight:
                changeTankRotation(ent, PI/4f);
                break;
            default:
                moving = false;
                break;
        }

        tankData.Direction = moving ? (new Vector2(MathF.Cos(ent.Rotation), MathF.Sin(ent.Rotation))) : Vector2.Zero;
        tankData.Collider.Velocity = tankData.Direction * tankData.Speed;
        tankData.Collider.Position = ent.Position - ent.DrawOffset / 2;

        float distance = 0f;
        Vector2 contact = new(), normal = new();
        var enemy = (Enemy)entities.First(a => a is Enemy);
        var enemyRect = enemy.TankData.Collider;

        if (Collision.DynamicRectVsRect(tankData.Collider, enemyRect, ref contact, ref normal, ref distance) && distance <= 1f)
        {
            var vel = tankData.Collider.Velocity;
            tankData.Collider.Velocity += (normal * new Vector2(MathF.Abs(vel.X), MathF.Abs(vel.Y)) * (1f - distance));
        }

        ent.Position += tankData.Collider.Velocity * Global.DELTA_TIME; 
        tankData.Collider.Position = ent.Position - ent.DrawOffset / 2;
        tankData.Barrel.Position = ent.Position;
    }

    private static void changeTankRotation(Entity tank, float targetRotation)
    {
        var canFlip180 = (float rotation) =>
        {
            bool greaterThanLowBoundRot = rotation > tank.Rotation - EPSILON;
            bool lessThanHighBoundRot = rotation < tank.Rotation + EPSILON;
            if (greaterThanLowBoundRot && lessThanHighBoundRot)
            {
                return true;
            }
            return false;
        };
        
        float rotationPlusPiWrap = Global.WrapRotation(targetRotation + MathHelper.Pi);
        if (canFlip180(rotationPlusPiWrap))
        {
            tank.Rotation = Global.WrapRotation(targetRotation);
        }
        else if (targetRotation + MathHelper.Pi > MathHelper.TwoPi)
        {
            var adjustedMaxRotationBounds = rotationPlusPiWrap;
            if (tank.Rotation > targetRotation || tank.Rotation < adjustedMaxRotationBounds)
                tank.Rotation -= TANK_ROT_SPEED;
            else
                tank.Rotation += TANK_ROT_SPEED;
        }
        else
        {
            if (tank.Rotation > targetRotation && tank.Rotation < targetRotation + MathHelper.Pi)
                tank.Rotation -= TANK_ROT_SPEED;
            else
                tank.Rotation += TANK_ROT_SPEED;
        }
        
        tank.Rotation = Global.WrapRotation(tank.Rotation);
        if (MathF.Abs(targetRotation - tank.Rotation) < EPSILON)
        {
            tank.Rotation = targetRotation;
        }
    }
}