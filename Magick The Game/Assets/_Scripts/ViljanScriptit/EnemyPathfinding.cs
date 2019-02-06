using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    Grid grid;
    public Transform seeker, target;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        //add the start node to openNodes
        List<Node> openNodes = new List<Node>();
        HashSet<Node> closedNodes = new HashSet<Node>();
        openNodes.Add(startNode);

        while (openNodes.Count > 0)
        {
            Node currentNode = openNodes[0];
            for (int i = 1; i < openNodes.Count; i++)
            {
                //change current node to one with the lowest f_cost
                if (openNodes[i].fCost < currentNode.fCost || openNodes[i].fCost == currentNode.fCost)
                {
                    if (openNodes[i].hCost < currentNode.hCost)
                        currentNode = openNodes[i];
                }
            }

            //remove currentNode from openNodes and add it to closedNodes
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            //check if currentNode is the targetNode
            if (currentNode == targetNode)
            {
                BacktrackPath(startNode, targetNode);
                return;
            }

            //check if the neighbours are walkable
            foreach (Node neighbour in grid.GetNeighbouringNodes(currentNode))
            {
                if (!neighbour.walkable || closedNodes.Contains(neighbour))
                {
                    continue;
                }

                //set new cost to the neighbour
                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openNodes.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openNodes.Contains(neighbour))
                        openNodes.Add(neighbour);
                }
            }
        }
    }

    void BacktrackPath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }
        else
        {
            return 14 * distanceX + 10 * (distanceY - distanceX);
        }
    }
}
