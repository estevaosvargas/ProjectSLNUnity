using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathGrid : MonoBehaviour
{
    public Dictionary<Vector2, Node> tiles = new Dictionary<Vector2, Node>();

    private void Awake()
    {
        Game.PathGrid = this;
    }

    public void OnDrawGizmos()
    {
        foreach (Node n in tiles.Values.ToArray())
        {
            Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(0, 1, n.movementPenalty));
            Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
            Gizmos.DrawCube(new Vector3(n.worldPosition.x + 0.5f, n.worldPosition.y, n.worldPosition.z + 0.5f), Vector3.one);
        }
    }
}