using Engine;
using Engine.Graphics;

namespace Script;

public static class PlayerController
{
    public static void UpdatePlayer(Player player)
    {
        var kState = Global.K_State;
        var mouseWorldPos = Renderer.Get().GetWorldMousePos();
        player.Barrel.Direction = Vector2.Normalize(mouseWorldPos - player.Barrel.Position);

        if (kState.IsKeyDown(Keys.A) && kState.IsKeyDown(Keys.W))
        {
            player.TankDir = TankDir.UpLeft;
        }
        else if (kState.IsKeyDown(Keys.D) && kState.IsKeyDown(Keys.W))
        {
            player.TankDir = TankDir.UpRight;
        }
        else if (kState.IsKeyDown(Keys.A) && kState.IsKeyDown(Keys.S))
        {
            player.TankDir = TankDir.DownLeft;
        }
        else if (kState.IsKeyDown(Keys.D) && kState.IsKeyDown(Keys.S))
        {
            player.TankDir = TankDir.DownRight;
        }
        else if (kState.IsKeyDown(Keys.A))
        {
            player.TankDir = TankDir.Left;
        }
        else if (kState.IsKeyDown(Keys.D))
        {
            player.TankDir = TankDir.Right;
        }
        else if (kState.IsKeyDown(Keys.W))
        {
            player.TankDir = TankDir.Up;
        }
        else if (kState.IsKeyDown(Keys.S))
        {
            player.TankDir = TankDir.Down;
        }
        else
        {
            player.TankDir = (TankDir)(-1);
        }

        if (Global.LeftMouseClicked())
        {
            GameEntities.TriggerEntityOnShoot(player);
        }
    }
}