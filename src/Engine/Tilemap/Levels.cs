using DotTiled;
using DotTiled.Serialization;
using Engine.Collisions;
using System.Collections.Generic;

namespace Engine.Tilemap;

static class Levels
{
    public static readonly Loader Loader;

    static Levels()
    {
        Loader = Loader.Default();        
    }
    
    public static void DrawLayers(MapData data, string[] visualLayerNames)
    {
        foreach (var layerName in visualLayerNames)
        {
            var layer = data.VisualLayers.OfType<TileLayer>().Single(l => l.Name == layerName);
            drawLayer(data, layer);
        }
    }

    private static void drawLayer(MapData data, TileLayer layer)
    {
        int y, x;
        uint tileGID;
        for (int i = 0; i < layer.Width * layer.Height; i++)
        {
            y = i / layer.Width;
            x = i % layer.Width;
            tileGID = layer.GetGlobalTileIDAtCoord(x, y);
            if (tileGID == 0)
                continue;
            
            var tileset = data.Map.ResolveTilesetForGlobalTileID(tileGID, out var localTileID);
            var srcRect = tileset.GetSourceRectangleForLocalTileID(localTileID);
            var monoGameSrc = new Rectangle(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height);
            var destRect = new Rectangle(x * tileset.TileWidth, y * tileset.TileHeight, tileset.TileWidth, tileset.TileHeight);

            Global.Batch.Draw
            (
                data.TilesetTextures[tileset.Image.Value.Source.Value],
                destRect,
                monoGameSrc,
                Color.White
            );
        }
    }

    public static Dictionary<string, Texture2D> LoadTilesetTextures(Map map)
    {
        return map.Tilesets.ToDictionary
        (
            tileset => tileset.Image.Value.Source.Value,
            tileset => Texture2D.FromFile(Global.Graphics.GraphicsDevice, 
                $"{Global.ASSETS_PATH}/png/{tileset.Image.Value.Source.Value}")
        );
    }

    public static void Unload(Dictionary<string, Texture2D> tilesetTextures)
    {
        foreach (var kvp in tilesetTextures)
        {
            kvp.Value.Dispose();
        }
    }
}