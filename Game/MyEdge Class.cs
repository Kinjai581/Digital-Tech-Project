using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;



namespace MonoGame_Files {
    public class Node
    {
        public Vector2 Position { get; set; }
        public Node Parent { get; set; }
        public float G { get; set; } // Cost from start to this node
        public float H { get; set; } // Heuristic cost from this node to goal
        public float F => G + H; // Total cost

        public Node(Vector2 position, Node parent, float g, float h)
        {
            Position = position;
            Parent = parent;
            G = g;
            H = h;
        }
    }
}
