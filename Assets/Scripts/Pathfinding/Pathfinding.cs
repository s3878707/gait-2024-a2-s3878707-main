// Adapted from: https://github.com/SebLague/Pathfinding-2D

using System;
using System.Collections.Generic;
using Globals;
using SteeringCalcs;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static AStarGrid grid;
    static Pathfinding instance;
    public float waterCostMultiplier;

    // For path smoothing. (Note: You may need to add more variables here.)
    public bool UsePathSmoothing;
    public LayerMask ObstacleMask;
    public float CircleCastRadius;
    public LayerMask WaterMask;

    void Awake()
    {
        grid = GetComponent<AStarGrid>();
        instance = this;
    }

    public static Node[] RequestPath(Vector2 from, Vector2 to)
    {
        return instance.FindPath(from, to);
    }

    Node[] FindPath(Vector2 from, Vector2 to)
    {
        Node[] waypoints = new Node[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(from);
        Node targetNode = grid.NodeFromWorldPoint(to);
        startNode.parent = startNode;

        if (!startNode.walkable)
        {
            startNode = grid.ClosestWalkableNode(startNode);
        }
        if (!targetNode.walkable)
        {
            targetNode = grid.ClosestWalkableNode(targetNode);
        }

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    float newMovementCostToNeighbour =
                        currentNode.gCost + GCost(currentNode, neighbour);

                    if (
                        newMovementCostToNeighbour < neighbour.gCost
                        || !openSet.Contains(neighbour)
                    )
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = Heuristic(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }

        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }

        if (UsePathSmoothing)
        {
            waypoints = SmoothPath(waypoints);
        }

        return waypoints;
    }

    Node[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Node[] waypoints = path.ToArray();
        Array.Reverse(waypoints);
        return waypoints;
    }

    private Node[] SmoothPath(Node[] path)
    {
        // TODO: Implement me properly!
        if (path.Length > 0)
        {
            List<Node> waypoints = new List<Node>();
            waypoints.Add(path[0]);
            Node currentNode = path[0];
            Node endNode = path[path.Length - 1];
            int currIdx = 0;

            while ((endNode.worldPosition - currentNode.worldPosition).magnitude > 0.001f)
            {
                // CircleCast for on lane node
                if (!currentNode.underwater)
                {
                    bool findPath = false;
                    for (int i = path.Length - 1; i > currIdx; i--)
                    {
                        Node checkNode = path[i];
                        Vector2 offset = checkNode.worldPosition - currentNode.worldPosition;
                        RaycastHit2D hit = Physics2D.CircleCast(
                            currentNode.worldPosition,
                            CircleCastRadius,
                            offset.normalized,
                            offset.magnitude,
                            ObstacleMask
                        );

                        RaycastHit2D waterHit = Physics2D.CircleCast(
                            currentNode.worldPosition,
                            CircleCastRadius,
                            offset.normalized,
                            offset.magnitude,
                            WaterMask
                        );

                        if (!waterHit && !hit)
                        {
                            waypoints.Add(checkNode);
                            currentNode = checkNode;
                            currIdx = i;
                            findPath = true;
                            break;
                        }
                    }
                    // move from lane to water
                    if (findPath == false)
                    {
                        currentNode = path[currIdx + 1];
                        waypoints.Add(path[currIdx + 1]);
                        currIdx = currIdx + 1;
                    }
                }
                else
                {
                    //CircleCast for underwater node
                    bool findPath = false;
                    int waterTargetIdx = 0;

                    // check if the destination is under water or on lane, if on lane, get out as quick as possible
                    for (int i = currIdx; i < path.Length; i++)
                    {
                        if (path[i].underwater)
                        {
                            waterTargetIdx = path.Length - 1;
                        }
                        else
                        {
                            waterTargetIdx = i - 1;
                            break;
                        }
                    }

                    for (int i = waterTargetIdx; i > currIdx; i--)
                    {
                        Node checkNode = path[i];
                        Vector2 offset = checkNode.worldPosition - currentNode.worldPosition;
                        RaycastHit2D hit = Physics2D.CircleCast(
                            currentNode.worldPosition,
                            CircleCastRadius,
                            offset.normalized,
                            offset.magnitude,
                            ObstacleMask
                        );
                        if (!hit)
                        {
                            waypoints.Add(checkNode);
                            currentNode = checkNode;
                            currIdx = i;
                            findPath = true;
                            break;
                        }
                        findPath = false;
                    }
                    //move from water to lane
                    if (findPath == false)
                    {
                        currentNode = path[currIdx + 1];
                        waypoints.Add(path[currIdx + 1]);
                    }
                }
            }
            return waypoints.ToArray();
        }
        return path;
    }

    private float GCost(Node nodeA, Node nodeB)
    {
        if (nodeB.underwater)
        {
            return waterCostMultiplier;
        }
        else
        {
            return 1.0f;
        }
    }

    private float Heuristic(Node nodeA, Node nodeB)
    {
        // Manhattan distance
        return Mathf.Abs(nodeB.gridX - nodeA.gridX) + Mathf.Abs(nodeB.gridY - nodeA.gridY);
    }
}
