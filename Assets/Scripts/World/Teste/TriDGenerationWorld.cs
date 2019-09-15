using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkTile
{
    public Vector3 Pos;
    public float creationTime;
    public GameObject theTile;

    public ChunkTile(GameObject thistile, float time)
    {
        theTile = thistile;
        creationTime = time;
    }
}

public class TriDGenerationWorld : MonoBehaviour
{
    public GameObject plane;
    public GameObject player;

    public int planeSize = 10;
    public int halfTilex = 10;
    public int halfTiley = 10;
    public int halfTilez = 10;

    public static float Seed = 0;
    public float DisRenderDistance = 10;
    Vector3 startPos;
    float updateTime = 0;
    Dictionary<string, ChunkTile> Tiles = new Dictionary<string,ChunkTile>();
    void Start()
    {
        Seed = Random.Range(-999, 999);
    }

    void Update()
    {
        int xMove = (int)(player.transform.position.x - startPos.x);
        int yMove = (int)(player.transform.position.y - startPos.y);
        int zMove = (int)(player.transform.position.z - startPos.z);

        if (Mathf.Abs(xMove) >= planeSize || Mathf.Abs(zMove) >= planeSize || Mathf.Abs(yMove) >= planeSize)
        {
            updateTime = Time.realtimeSinceStartup;

            int playerX = (int)(Mathf.Floor(player.transform.position.x / planeSize) * planeSize);
            int playerY = (int)(Mathf.Floor(player.transform.position.y / planeSize) * planeSize);
            int playerZ = (int)(Mathf.Floor(player.transform.position.z/ planeSize) * planeSize);

            for (int x = -halfTilex; x < halfTilex; x++)
            {
                for (int z = -halfTilez; z < halfTilez; z++)
                {
                    Vector3 pos = new Vector3(x * planeSize + playerX, 0, z * planeSize + playerZ);

                    string tilename = "Tile_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.y)).ToString() + "_" + ((int)(pos.z)).ToString();

                    if (!Tiles.ContainsKey(tilename))
                    {
                        GameObject t = (GameObject)Instantiate(plane, pos, Quaternion.identity);

                        t.name = tilename;
                        ChunkTile tile = new ChunkTile(t, updateTime);
                        Tiles.Add(tilename, tile);
                    }
                    else
                    {
                        (Tiles[tilename] as ChunkTile).creationTime = updateTime;
                    }

                    /*for (int y = -halfTiley; y < halfTiley; y++)
                    {
                        
                    }*/
                }
            }

            /*Hashtable newTerrain = new Hashtable();
            foreach (ChunkTile til in Tiles.Values)
            {
                if (til.creationTime != updateTime)
                {
                    string nametile = til.theTile.name;
                    Destroy(til.theTile);
                    //Tiles.Remove(nametile);
                }
                else
                {
                    newTerrain.Add(til.theTile.name, til);
                }
            }

            Tiles = newTerrain;*/
            DeleteChunk();
            startPos = player.transform.position;
        }
    }

    void DeleteChunk()
    {
        List<ChunkTile> DeleteChuks = new List<ChunkTile>(Tiles.Values);
        Queue<ChunkTile> deletechuks = new Queue<ChunkTile>();

        for (int i = 0; i < DeleteChuks.Count; i++)
        {
            //float Distanc = Vector3.Distance(transform.position, DeleteChuks[i].theTile.transform.position);

            if (DeleteChuks[i].creationTime != updateTime)
            {
                deletechuks.Enqueue(DeleteChuks[i]);
            }

            while (deletechuks.Count > 0)
            {
                ChunkTile chuks = deletechuks.Dequeue();
                Tiles.Remove(chuks.theTile.name);
                Destroy(chuks.theTile);
            }
        }
    }
}