using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

public class Pathfindingentity : EntityLife
{
    public Vector3 target;
    const float minPathUpdateTime = .2f;
    const float pathUpdateMoveThreshold = .5f;
    public float speed = 20;
    public bool HaveTarget = false;
    Vector3[] path = new Vector3[0] { };
    Vector3 Targetnode;

    public void OnDrawGizmos()
    {
        /*Gizmos.color = Color.black;
        if (path.Length > 0)
        {
            foreach (Vector3 p in path)
            {
                Gizmos.DrawCube(new Vector3(p.x + 0.5f, p.y + 0.5f, p.z + 1), Vector3.one);
            }
        }*/
    }

    public void Run(Vector3 Target)
    {
        target = Target;
        HaveTarget = true;
        StartCoroutine(UpdatePath());
    }

    public void Stop()
    {
        StopCoroutine("FollowPath");
        HaveTarget = false;
        StopCoroutine(UpdatePath());
        path = null;
        target = Vector3.zero;
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = waypoints;

            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator UpdatePath()
    {
        Game.GameManager.RequestPath(new GameManager.PathRequest(transform.position, target, OnPathFound), this);

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target;

        while (HaveTarget)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            //print(((target.position - targetPosOld).sqrMagnitude) + "    " + sqrMoveThreshold);
            if (HaveTarget)
            {
                if ((target - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    Game.GameManager.RequestPath(new GameManager.PathRequest(transform.position, target, OnPathFound), this);
                    targetPosOld = target;
                }
            }
        }
    }

    IEnumerator FollowPath()
    {
        float currentWaypointX = path[0].x;
        float currentWaypointZ = path[0].z;

        bool follow = true;
        int targetIndex = 0;
        while (follow)
        {
            if (new Vector3(transform.position.x, 0, transform.position.z) == new Vector3(currentWaypointX, 0, currentWaypointZ))
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypointX = path[targetIndex].x;
                currentWaypointZ = path[targetIndex].z;
            }

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentWaypointX, 0, currentWaypointZ), speed * Time.deltaTime);
            yield return null;

            if (new Vector3(transform.position.x, 0, transform.position.z) == Targetnode)
            {
                follow = false;
            }
        }
    }

    public void FindPath(GameManager.PathRequest request, Action<GameManager.PathResult> callback)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = GetTileAt((int)request.pathStart.x, (int)request.pathStart.z);
        Node targetNode = GetTileAt((int)request.pathEnd.x, (int)request.pathEnd.z);

        if (startNode != null && targetNode != null)
        {
            startNode.parent = startNode;
            Targetnode = targetNode.worldPosition;

            if (startNode.walkable && targetNode.walkable)
            {
                Heap<Node> openSet = new Heap<Node>(Game.PathGrid.tiles.Count);
                HashSet<Node> closedSet = new HashSet<Node>();
                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        sw.Stop();
                        //print("Path found: " + sw.ElapsedMilliseconds + " ms");
                        pathSuccess = true;
                        break;
                    }

                    foreach (Node neighbour in GetNeighboors(currentNode))
                    {
                        if (!neighbour.walkable || closedSet.Contains(neighbour))
                        {
                            continue;
                        }

                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;

                            if (!openSet.Contains(neighbour))
                                openSet.Add(neighbour);
                            else
                                openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode);
                pathSuccess = waypoints.Length > 0;
            }
            callback(new GameManager.PathResult(waypoints, pathSuccess, request.callback));
        }
    }

    public List<Node> GetNeighboors(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0)
                    continue;

                int checkX = (int)node.worldPosition.x + x;
                int checkZ = (int)node.worldPosition.z + z;

                if (Game.PathGrid.tiles.ContainsKey(new Vector2(checkX, checkZ)))
                {
                    neighbours.Add(Game.PathGrid.tiles[new Vector2(checkX, checkZ)]);
                }
            }
        }

        return neighbours;
    }

    public Node GetTileAt(int x, int z)
    {
        if (Game.PathGrid.tiles.ContainsKey(new Vector2(x, z)))
        {
            return Game.PathGrid.tiles[new Vector2(x, z)];
        }
        return null;
    }

    #region PathFinding

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;

    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            waypoints.Add(path[i].worldPosition + new Vector3(0.5f, 0, 0.5f));
            /*Vector2 directionNew = new Vector2(path[i - 1].worldPosition.x - path[i].worldPosition.x, path[i - 1].worldPosition.y - path[i].worldPosition.y);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;*/
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs((int)nodeA.worldPosition.x - (int)nodeB.worldPosition.x);
        int dstZ = Mathf.Abs((int)nodeA.worldPosition.z - (int)nodeB.worldPosition.z);

        if (dstX > dstZ)
            return 14 * dstZ + 10 * (dstX - dstZ);
        return 14 * dstX + 10 * (dstZ - dstX);
    }
    #endregion
}

public class Heap<T> where T : IHeapItem<T>
{

    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }

            }
            else
            {
                return;
            }

        }
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}

public class Node : IHeapItem<Node>
{

    public bool walkable;
    public Vector3 worldPosition;
    public int movementPenalty;

    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;

    public Node(bool _walkable, Vector3 _worldPos, int _penalty)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        movementPenalty = _penalty;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}