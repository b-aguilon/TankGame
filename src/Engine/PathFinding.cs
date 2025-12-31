using System.Collections.Generic;
using Engine.Graphics;

namespace Engine;

public static class PathFinder2
{
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

public static class PathFinder
{
    private static NavNode[] grid;
    private static int mapWidth = 16;
    private static int mapHeight = 16;
    private static NavNode start;
    private static NavNode end;

    public static void Create()
    {
        grid = new NavNode[mapWidth * mapHeight];

        for (int i = 0; i < grid.Length; i++)
        {
            int x = i % mapWidth;
            int y = i / mapWidth;

            grid[y * mapWidth + x] = new NavNode();
            grid[y * mapWidth + x].Neighbours = new List<NavNode>();
            grid[y * mapWidth + x].X = x;
            grid[y * mapWidth + x].Y = y;
            grid[y * mapWidth + x].Obstacle = false;
            grid[y * mapWidth + x].Visited = false;
            grid[y * mapWidth + x].Parent = null;
        }


        for (int i = 0; i < grid.Length; i++)
        {
            int x = i % mapWidth;
            int y = i / mapWidth;
            var node = grid[y * mapWidth + x];

            var leftIndex = y * mapWidth + x - 1;
            var rightIndex = y * mapWidth + x + 1;
            var upIndex = (y - 1) * mapWidth + x;
            var downIndex = (y + 1) * mapWidth + x;

            if (y > 0)
            {
                node.Neighbours.Add(grid[upIndex]);
            }
            if (y < mapHeight - 1)
            {
                node.Neighbours.Add(grid[downIndex]);
            }
            if (x > 0)
            {
                node.Neighbours.Add(grid[leftIndex]);
            }
            if (x < mapWidth - 1)
            {
                node.Neighbours.Add(grid[rightIndex]);
            }
        }

        start = grid[0];
        end = grid[grid.Length - 1];
    }

    public static void Draw()
    {
        var mousePos = Renderer.Get().GetScreenMousePos();

        const int nodeSize = 9;
        const int nodeBorderIncrement = 2;
        int nodeBorderX = 0;
        int nodeBorderY = 0;

        int selectedNodeX = (int)mousePos.X / (nodeSize + nodeBorderIncrement);
        int selectedNodeY = (int)mousePos.Y / (nodeSize + nodeBorderIncrement);

        int halfSize = (nodeSize + nodeBorderIncrement) / 2;
        int fullSize = nodeSize + nodeBorderIncrement;

        if(Global.LeftMouseClicked())
        {
            if (selectedNodeX >= 0 && selectedNodeX < mapWidth)
            {
                if (selectedNodeY >= 0 && selectedNodeY < mapHeight)
                {
                    var node = grid[selectedNodeY * mapWidth + selectedNodeX];
                    node.Obstacle = !node.Obstacle;

                    solveAStar();
                }
            }
        }

        //draw neighbour connections
        for (int i = 0; i < grid.Length; i++)
        {
            int x = i % mapWidth;
            int y = i / mapWidth;
            var current = grid[y * mapWidth + x];

            foreach (var n in current.Neighbours)
            {
                Renderer.DrawLine(new(current.X*fullSize + halfSize, current.Y*fullSize + halfSize), new(n.X*fullSize + halfSize, n.Y*fullSize + halfSize), Color.Blue);
            }
        }

        //draw nodes
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                var node = grid[y * mapWidth + x];
                var colour = Color.Blue;
                if (node.Obstacle)
                {
                    colour = Color.Black;
                }
                else if (node == end)
                {
                    colour = Color.Red;
                }
                else if (node == start)
                {
                    colour = Color.Green;
                }
                if (node.Visited)
                {
                    colour *= 0.5f;
                }
                Renderer.DrawRectangle(new Rectangle(x*nodeSize + nodeBorderX, y*nodeSize + nodeBorderY, nodeSize, nodeSize), colour);
                nodeBorderX += nodeBorderIncrement;
            }
            nodeBorderX = 0;
            nodeBorderY += nodeBorderIncrement;
        }

        //draw path
        if (end != null)
        {
            NavNode p = end;
            while(p.Parent != null)
            {
                Renderer.DrawLine(new(p.X*fullSize + halfSize, p.Y*fullSize + halfSize), new(p.Parent.X*fullSize + halfSize, p.Parent.Y*fullSize + halfSize), Color.White);
                p = p.Parent;
            }
        }
    }

    private static void solveAStar()
    {
        for (int i = 0; i < grid.Length; i++)
        {
            int y = i / mapWidth;
            int x = i % mapWidth;
            var node = grid[y * mapWidth + x];
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
    }
}

public class NavNode
{
    public bool Obstacle = false;
    public bool Visited = false;
    public float GlobalDistance;
    public float LocalDistance;
    public int X, Y;
    public List<NavNode> Neighbours;
    public NavNode Parent;
}