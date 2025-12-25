using DotTiled;

using System.Collections.Generic;

namespace Engine.Tilemap;

struct MapData
{
    public Map Map {get; init;}
    public IEnumerable<TileLayer> VisualLayers {get; init;}
    public Dictionary<string, Texture2D> TilesetTextures {get; init;}
}