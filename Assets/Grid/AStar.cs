using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Pathfinding_AStar
{
    [System.Serializable]
    class NodeInformation
    {
        public GridTile node;
        public NodeInformation parent;
        public float gCost;
        public float hCost;
        public float fCost;

        public NodeInformation(GridTile node, NodeInformation parent, float gCost, float hCost)
        {
            this.node = node;
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
            fCost = gCost + hCost;
        }

        public void UpdateNodeInformation(NodeInformation parent, float gCost, float hCost)
        {
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
            fCost = gCost + hCost;
        }
    }

    private bool _cutCorners;
    private bool _allowDiagonal;

    private List<Vector3> _path;
    public List<Vector3> Path { get { return _path; } }

    public Pathfinding_AStar(bool allowDiagonal, bool cutCorners)
    {
        _cutCorners = cutCorners;
        _allowDiagonal = allowDiagonal;
    }

    public void GeneratePath(GridTile start, GridTile end)
    {
        List<NodeInformation> openList = new List<NodeInformation>();
        List<NodeInformation> closedList = new List<NodeInformation>();
        List<NodeInformation> pathNodes = new List<NodeInformation>();
        List<Vector3> path = new List<Vector3>();

        //My garbage here :D 

        NodeInformation startNode = new NodeInformation(start, null, 0f, 0f);

        NodeInformation currentNode = startNode;

        //Adds starting node to open
        openList.Add(startNode);

        int iterations = 0;
        int maxIterations = 2000;
        while (iterations < maxIterations)
        {
            iterations++;
            if (openList.Count == 0) break; // Breaks if no path is found.

            //Gets Lowest fCost;
            currentNode = openList[0];

            foreach (NodeInformation nodeInfo in openList)
            {
                if (nodeInfo.fCost < currentNode.fCost)
                {
                    currentNode = nodeInfo;
                }
            }

            if (currentNode.node == end) break; // Breaks when the current node is the end node.

            // Checks if Orthogonal tiles are present

            bool orthogonalPresent = false;

            if (!_cutCorners && _allowDiagonal)
            {
                for (int i = 0; i < currentNode.node.Neighbours.Length; i++)
                {
                    GridTile node = currentNode.node.Neighbours[i];
                    if (!node) continue;

                    if (i % 2 == 0 && (node.tileType == TileType.unwalkable))
                    {
                        orthogonalPresent = true;
                        break;
                    }
                }
            }

            //Iterates through all neighours

            for (int i = 0; i < currentNode.node.Neighbours.Length; i++)
            {
                GridTile node = currentNode.node.Neighbours[i];

                if (!node) continue; //Skips non existant node;

                //Cuts corners or skips diagonals depending on settings.
                bool doesDiagonal = ((!_allowDiagonal || orthogonalPresent) && i % 2 == 1);

                //Skips the diagonal neighbour if diagonal's are disabled
                if (doesDiagonal || (node.tileType == TileType.unwalkable)) continue;
                if (node.TileEntity) continue;

                //Checks if node is in closed list and then continues loop if found.
                NodeInformation foundNode = null;

                float newG = currentNode.gCost + Vector3.Distance(currentNode.node.transform.position, node.transform.position);
                float newH = Vector3.Distance(node.transform.position, end.transform.position);

                foreach (NodeInformation nodeInfo in openList)
                {
                    if (nodeInfo.node == node)
                    {
                        foundNode = nodeInfo;
                        break;
                    }
                }

                foreach (NodeInformation nodeInfo in openList)
                {
                    if (foundNode != null) break;

                    if (nodeInfo.node == node)
                    {
                        foundNode = nodeInfo;
                        break;
                    }
                }

                if (foundNode == null)
                {
                    NodeInformation newNode = new NodeInformation(node, currentNode, newG, newH);
                    openList.Add(newNode);
                }
                else
                {
                    if (foundNode.fCost > newG + newH)
                    {
                        foundNode.UpdateNodeInformation(currentNode, newG, newH);
                    }
                }

            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);
        }

        //Backtracking

        while (currentNode != null)
        {
            pathNodes.Add(currentNode);
            currentNode = currentNode.parent;
        }

        pathNodes.Reverse();

        foreach (NodeInformation node in pathNodes)
        {
            if (!node.node) continue;

            node?.node?.SetColour(Color.green);
            path.Add(node.node.transform.position);
        }

        end.SetColour(Color.red);

        _path = path;
    }
}

