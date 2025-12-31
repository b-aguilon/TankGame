using DotTiled;

using System.Collections.Generic;

namespace Engine.Tilemap;

class NavNode
{
    public bool Obstacle = false;
    public bool Visited = false;
    public float GlobalDistance;
    public float LocalDistance;
    public int X, Y;
    public List<NavNode> Neighbours;
    public NavNode Parent;
}

struct MapData
{
    public Map Map {get; init;}
    public IEnumerable<TileLayer> VisualLayers {get; init;}
    public ObjectLayer CollisionLayer {get; init;}
    public NavNode[] NavGrid {get; init;}
    public Dictionary<string, Texture2D> TilesetTextures {get; init;}
}