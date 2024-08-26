using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;



namespace MonoGame_Files {
    
    public class Node
    {
        public Vector2 Position { get; set; }
        public Node Parent { get; set; }
        public float G { get; set; } // Cost from start to current node
        public float H { get; set; } // Estimated cost from current node to end
        public float F => G + H; // Total cost

        public Node(Vector2 position)
        {
            Position = position;
        }
    }
    

    public class AStar
    {
        private int[,] grid;
        private Vector2 start;
        private Vector2 end;

        public AStar(int[,] grid, Vector2 start, Vector2 end)
        {
            this.grid = grid;
            this.start = start;
            this.end = end;
        }

        public List<Vector2> FindPath()
        {
            List<Node> openList = new List<Node>();
            HashSet<Vector2> closedSet = new HashSet<Vector2>();

            Node startNode = new Node(start);
            Node endNode = new Node(end);
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                Node currentNode = openList[0];
                foreach (var node in openList)
                {
                    if (node.F < currentNode.F || (node.F == currentNode.F && node.H < currentNode.H))
                    {
                        currentNode = node;
                    }
                }

                openList.Remove(currentNode);
                closedSet.Add(currentNode.Position);

                if (currentNode.Position == endNode.Position)
                {
                    return ReconstructPath(currentNode);
                }

                foreach (Vector2 neighborPos in GetNeighbors(currentNode.Position))
                {
                    if (closedSet.Contains(neighborPos) || grid[(int)neighborPos.X, (int)neighborPos.Y] == 1)
                    {
                        continue;
                    }

                    Node neighborNode = new Node(neighborPos)
                    {
                        Parent = currentNode,
                        G = currentNode.G + 1,
                        H = GetDistance(neighborPos, endNode.Position)
                    };

                    if (openList.Exists(node => node.Position == neighborPos && node.G <= neighborNode.G))
                    {
                        continue;
                    }

                    openList.Add(neighborNode);
                }
            }

            return null; // No path found
        }

        private List<Vector2> ReconstructPath(Node node)
        {
            List<Vector2> path = new List<Vector2>();
            while (node != null)
            {
                path.Add(node.Position);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }

        private List<Vector2> GetNeighbors(Vector2 position)
        {
            List<Vector2> neighbors = new List<Vector2>
        {
            new Vector2(position.X - 1, position.Y),
            new Vector2(position.X + 1, position.Y),
            new Vector2(position.X, position.Y - 1),
            new Vector2(position.X, position.Y + 1)
        };

            neighbors.RemoveAll(n => n.X < 0 || n.Y < 0 || n.X >= grid.GetLength(0) || n.Y >= grid.GetLength(1));
            return neighbors;
        }

        private float GetDistance(Vector2 a, Vector2 b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
    }
}

