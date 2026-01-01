using System.Collections.Generic;

namespace Engine;

public abstract class Entity
{
    public Texture2D Texture;
    public Color Tint = Color.White;
    public Rectangle Source;
    public Rectangle Destination => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
    public Vector2 Position = Vector2.Zero;
    public Vector2 RotationPivot = Vector2.Zero;
    public Vector2 DrawOffset = Vector2.Zero;
    public float Rotation = 0f;
    public float LayerDepth = 0f;
    public int Width = 1, Height = 1;
}

public static class Entities
{
    private static readonly List<Entity> entities = new List<Entity>();

    public static IEnumerable<Entity> GetEntities() => entities;

    private static event EventHandler<List<Entity>> entityAdded;
    private static event EventHandler<List<Entity>> entityRemoved;

    public static void TriggerAddEntity(Entity entity)
    {
        entityAdded?.Invoke(entity, entities);
    }

    public static void TriggerRemoveEntity(Entity entity)
    {
        entityRemoved?.Invoke(entity, entities);
    }

    public static void TriggerRemoveAllEntities()
    {
        foreach (var ent in entities)
        {
            TriggerRemoveEntity(ent);
        }
    }

    public static void AddEntityAddedListener(EventHandler<List<Entity>> handler)
    {
        entityAdded += handler;
    }

    public static void RemoveEntityAddedListener(EventHandler<List<Entity>> handler)
    {
        entityAdded -= handler;
    }

    public static void AddEntityRemovedListener(EventHandler<List<Entity>> handler)
    {
        entityRemoved += handler;
    }

    public static void RemoveEntityRemovedListener(EventHandler<List<Entity>> handler)
    {
        entityRemoved -= handler;
    }

    public static void ClearEntityAddedListeners()
    {
        entityAdded = null;
    }

    public static void ClearEntityRemovedListeners()
    {
        entityRemoved = null;
    }

    public static void ClearEntities()
    {
        entities.Clear();
    }
}