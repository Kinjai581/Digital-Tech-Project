using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;



namespace MonoGame_Files {

public class MyNode
{
    public int Index { get; set; }
    public Vector2 Pos { get; private set; }
    public List<MyEdge> Edges { get; private set; }

    public MyNode parent = null;
    public double G = double.MaxValue;
    public double H = 0;
    public double F { get { return G + H; } }


    public MyNode(int x, int y)
    {
        Edges = new List<MyEdge>();
        Pos = new Vector2(x, y);
        Index = -1;
    }
    public MyNode(Vector2 pos)
    {
        Edges = new List<MyEdge>();
        Pos = pos;
        Index = -1;
    }

    public MyNode(int x, int y, int idx)
    {
        Edges = new List<MyEdge>();
        Pos = new Vector2(x, y);
        Index = idx;
    }
    public MyNode(Vector2 pos, int idx)
    {
        Edges = new List<MyEdge>();
        Pos = pos;
        Index = idx;
    }
}
public class MyEdge
{
    public int To { get; private set; }
    public int From { get; private set; }

    protected MyNode from;
    protected MyNode to;


    public MyEdge(int from, int to, MyGraph g)
    {
        From = from;
        To = to;
        this.from = g.GetNode(from);
        this.to = g.GetNode(to);
    }

    public MyNode NodeFrom()
    {
        return from;
    }

    public MyNode NodeTo()
    {
        return to;
    }
}

public class MyGraph
    {
        public List<MyNode> Nodes { get; private set; }

        public MyGraph()
        {
            Nodes = new List<MyNode>();
        }

        // Automatically populate nodes and edges based on a 2D matrix
        public void BuildGraphFromMatrix(int[,] matrix)
        {
            int size = matrix.GetLength(0);

            // Create nodes
            for (int i = 0; i < size; i++)
            {
                MyNode node = new MyNode(i, i, i); // Assuming the position is (i, i) for simplicity
                AddNode(node);
            }

            // Create edges
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (matrix[i, j] != 0)
                    {
                        AddEdge(i, j);
                    }
                }
            }
        }

        public MyNode GetNode(int index)
        {
            if (index >= 0 && index < Nodes.Count)
            {
                return Nodes[index];
            }
            return null;
        }

        public void AddNode(MyNode node)
        {
            node.Index = Nodes.Count; // Set the index of the node to its position in the list
            Nodes.Add(node);
        }

        public void AddEdge(int fromIndex, int toIndex)
        {
            MyNode fromNode = GetNode(fromIndex);
            MyNode toNode = GetNode(toIndex);

            if (fromNode != null && toNode != null)
            {
                MyEdge edge = new MyEdge(fromIndex, toIndex, this);
                fromNode.Edges.Add(edge);
            }
        }
    }
}
