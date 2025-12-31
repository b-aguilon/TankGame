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

    public static NavNode[] CreateNavGrid(int width, int height)
    {
        var grid = new NavNode[width * height];

        for (int i = 0; i < grid.Length; i++)
        {
            int x = i % width;
            int y = i / width;

            grid[y * width + x] = new NavNode();
            grid[y * width + x].Neighbours = new List<NavNode>();
            grid[y * width + x].X = x;
            grid[y * width + x].Y = y;
            grid[y * width + x].Obstacle = false;
            grid[y * width + x].Visited = false;
            grid[y * width + x].Parent = null;
        }


        for (int i = 0; i < grid.Length; i++)
        {
            int x = i % width;
            int y = i / width;
            var node = grid[y * width + x];

            var leftIndex = y * width + x - 1;
            var rightIndex = y * width + x + 1;
            var upIndex = (y - 1) * width + x;
            var downIndex = (y + 1) * width + x;

            if (y > 0)
            {
                node.Neighbours.Add(grid[upIndex]);
            }
            if (y < height - 1)
            {
                node.Neighbours.Add(grid[downIndex]);
            }
            if (x > 0)
            {
                node.Neighbours.Add(grid[leftIndex]);
            }
            if (x < width - 1)
            {
                node.Neighbours.Add(grid[rightIndex]);
            }
        }

        return grid;
    }

    public static List<(Rectangle rect, Vector2 dir)> FindPath(NavNode[] grid, int gridWidth, int nodeSize, NavNode start, NavNode end)
    {
        var rects = new List<(Rectangle rect, Vector2 dir)>();

        for (int i = 0; i < grid.Length; i++)
        {
            int y = i / gridWidth;
            int x = i % gridWidth;
            var node = grid[y * gridWidth + x];
            node.Visited = false;
            node.GlobalDistance = float.PositiveInfinity;
            node.LocalDistance = float.PositiveInfinity;
            node.Parent = null;
        }

        var distance = (NavNode a, NavNode b) =>
        {
            return MathF.Sqrt((a.X-b.X)*(a.X-b.X) + (a.Y-b.Y)*(a.Y-b.Y));
        };
        var heuristic = (NavNode a, NavNode b) =>
        {
            return distance(a, b);
        };

        NavNode current = start;
        start.LocalDistance = 0f;
        start.GlobalDistance = heuristic(start, end);

        List<NavNode> untested = [start];

        while (untested.Count > 0)
        {
            untested = untested.OrderBy(n => n.GlobalDistance).ToList();
            while (untested.Count > 0 && untested[0].Visited)
            {
                untested.RemoveAt(0);
            }

            if (untested.Count <= 0)
                break;

            current = untested[0];
            current.Visited = true;

            foreach (var node in current.Neighbours)
            {
                if (!node.Visited && !node.Obstacle)
                {
                    untested.Add(node);
                }

                float possiblyLowerDistance = current.LocalDistance + distance(current, node);

                if (possiblyLowerDistance < node.LocalDistance)
                {
                    node.Parent = current;
                    node.LocalDistance = possiblyLowerDistance;
                    node.GlobalDistance = node.LocalDistance + heuristic(node, end);
                }
            }
        }

        if (end != null)
        {
            NavNode p = end;
            while(p.Parent != null)
            {
                rects.Add((new(p.X * nodeSize, p.Y * nodeSize, nodeSize, nodeSize), new Vector2(p.X - p.Parent.X, p.Y - p.Parent.Y)));
                p = p.Parent;
            }
        }

        return rects;
    }
}