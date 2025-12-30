using System.Collections.Generic;
using Engine;
using Engine.Collisions;

namespace Script;

public static class TankController
{
    private const float TANK_ROT_SPEED = MathHelper.Pi / 70f;
    private const float EPSILON = MathHelper.Pi / 30f;

    public static void UpdateTank(Entity ent, TankData tankData, List<RectCollider> colliders)
    {
        changeTankDirection(ent, tankData);
        handleTankCollisions(ent, tankData, colliders);
        updateTankBarrel(ent, tankData);
    }

    private static void changeTankDirection(Entity ent, TankData tankData)
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

        if (moving)
        {
            tankData.Direction = new Vector2(MathF.Cos(ent.Rotation), MathF.Sin(ent.Rotation));
        }
        else
        {
            tankData.Direction = Vector2.Zero;
        }
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

    private static void handleTankCollisions(Entity ent, TankData tankData, List<RectCollider> colliders)
    {
        tankData.Collider.Velocity = tankData.Direction * tankData.Speed;
        tankData.Collider.Position = ent.Position - ent.DrawOffset / 2;

        Collision.HandleRectVsRectCollisions(tankData.Collider, colliders); 

        ent.Position += tankData.Collider.Velocity * Global.DELTA_TIME; 
        tankData.Collider.Position = ent.Position - ent.DrawOffset / 2;
    }

    private static void updateTankBarrel(Entity ent, TankData tankData)
    {
        tankData.Barrel.Rotation = MathF.Atan2(tankData.Barrel.Direction.Y, tankData.Barrel.Direction.X);
        tankData.Barrel.Position = ent.Position;
    }
}