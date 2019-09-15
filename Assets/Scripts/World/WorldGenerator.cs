using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class WorldGenerator : DCallBack
{
    public static WorldGenerator Instance;
    public Transform Player;
    public UnityStandardAssets._2D.Camera2DFollow Cam;
    public GameObject ChunkGO;
    public int RenderDistance;
    public int DisRenderDistance = 5;

    Dictionary<Vector2, Chunk> chunkMap;
    Dictionary<Vector2, bool> ClientchunkMap;
    public List<Chunk> ChunksList = new List<Chunk>();

    public Transform SlectedBlock;
    public WorldType CurrentWorld;
    public int Seed = 0;
    public Texture2D HeightTeste;

    [Header("TimeData")]
    public int h;
    public int d;
    public int m;

    void Awake()
    {
        Instance = this;
        chunkMap = new Dictionary<Vector2, Chunk>();
        ClientchunkMap = new Dictionary<Vector2, bool>();
#if Client
        Seed = Game.GameManager.Seed;

        Player = Game.GameManager.MyPlayer.MyObject.transform;
        Player.GetComponent<EntityPlayer>().World = this.transform;
        Cam.target = Game.GameManager.MyPlayer.MyObject.transform;
#endif
    }

    public override void OnRespawn()
    {
        Player = Game.GameManager.MyPlayer.MyObject.transform;
        Player.GetComponent<EntityPlayer>().World = this.transform;
        Cam.target = Game.GameManager.MyPlayer.MyObject.transform;

        base.OnRespawn();
    }

    void Start()
    {
        UpdateFindChunk();
    }


    public void UpdateFindChunk()
    {
        if (Game.GameManager.SinglePlayer)
        {
            FindChunksToLoad();
            DeleteChunk();
        }
        else if (Game.GameManager.MultiPlayer)
        {
            NetFindChunksToLoad();
            NetFindDeleteChunk();
        }
    }

    void Update()
    {
    #if Client
        if (MouselockFake.IsLock == false)
        {
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                if (Cam.GetComponent<Camera>().orthographicSize < 10)
                {
                    Cam.GetComponent<Camera>().orthographicSize += 1;
                }
            }

            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                if (Cam.GetComponent<Camera>().orthographicSize > 2)
                {
                    Cam.GetComponent<Camera>().orthographicSize -= 1;
                }
            }

            if (Input.GetKeyDown(KeyCode.KeypadMultiply))
            {
                Cam.GetComponent<Camera>().orthographicSize = 4.7f;
            }

            h = DataTime.Hora;
            d = DataTime.Dia;
            m = DataTime.Mes;
        }
    #endif
    }

    void FindChunksToLoad()
    {
        if (Player)
        {
            int xPos = (int)Player.position.x;
            int yPos = (int)Player.position.y;

            for (int i = -RenderDistance; i < RenderDistance; i++)
            {
                for (int y = -RenderDistance; y < RenderDistance; y++)
                {
                    MakeChunkAt(xPos + i, yPos + y);
                }
            }
        }

        /*for (int i = xPos - Chunk.Size; i < xPos + (2 * Chunk.Size); i += Chunk.Size)
        {
            for (int j = yPos - Chunk.Size; j < yPos + (2 * Chunk.Size); j += Chunk.Size)
            {
                MakeChunkAt(i, j);
            }
        }*/
    }

    void MakeChunkAt(int x, int y)
    {
        x = Mathf.FloorToInt(x / (float)Chunk.Size) * Chunk.Size;
        y = Mathf.FloorToInt(y / (float)Chunk.Size) * Chunk.Size;

        if (chunkMap.ContainsKey(new Vector2(x, y)) == false)
        {
            GameObject go = Instantiate(ChunkGO, new Vector3(x, y, 0), Quaternion.identity);

            chunkMap.Add(new Vector2(x, y), go.GetComponent<Chunk>());
        }
    }

    void DeleteChunk()
    {
        if (Player)
        {
            List<Chunk> DeleteChuks = new List<Chunk>(chunkMap.Values);
            Queue<Chunk> deletechuks = new Queue<Chunk>();

            for (int i = 0; i < DeleteChuks.Count; i++)
            {
                float Distanc = Vector3.Distance(Player.position, DeleteChuks[i].transform.position);

                if (Distanc > DisRenderDistance * Chunk.Size)
                {
                    deletechuks.Enqueue(DeleteChuks[i]);
                }

                while (deletechuks.Count > 0)
                {
                    Chunk chuks = deletechuks.Dequeue();
                    chunkMap.Remove(chuks.transform.position);
                    Destroy(chuks.gameObject);
                }
            }
        }
    }

    public Chunk GetChunkAt(int x, int y)
    {
        x = Mathf.FloorToInt(x / (float)Chunk.Size) * Chunk.Size;
        y = Mathf.FloorToInt(y / (float)Chunk.Size) * Chunk.Size;

        Vector2 chunkpos = new Vector2(x, y);

        if (chunkMap.ContainsKey(chunkpos))
        {
            return chunkMap[chunkpos];
        }
        else
        {
            return null;
        }
    }

    public Tile GetTileAt(int x, int y)
    {
        Chunk chunk = GetChunkAt(x, y);

        if (chunk != null)
        {
            return chunk.tiles[x - (int)chunk.transform.position.x, y - (int)chunk.transform.position.y];
        }
        return null;
    }

    public Tile GetTileAt(float x, float y)
    {
        int mx = Mathf.FloorToInt(x);
        int my = Mathf.FloorToInt(y);

        Chunk chunk = GetChunkAt(mx, my);

        if (chunk != null)
        {
            return chunk.tiles[mx - (int)chunk.transform.position.x, my - (int)chunk.transform.position.y];
        }
        return null;
    }

    public Vector3 GetPos(float x, float y)
    {
        int mx = Mathf.FloorToInt(x);
        int my = Mathf.FloorToInt(y);

        Chunk chunk = GetChunkAt(mx, my);

        if (chunk != null)
        {
            return new Vector3(mx - (int)chunk.transform.position.x, my - (int)chunk.transform.position.y, 0);
        }
        return Vector3.zero;
    }

    //<!---------------------------------------->

    void NetFindChunksToLoad()
    {
        if (Player)
        {
            int xPos = (int)Player.position.x;
            int yPos = (int)Player.position.y;

            int xx = 0;
            int yy = 0;

            for (int i = -RenderDistance; i < RenderDistance; i++)
            {
                for (int y = -RenderDistance; y < RenderDistance; y++)
                {
                    xx = xPos + i;
                    yy = yPos + y;

                    xx = Mathf.FloorToInt(xx / (float)Chunk.Size) * Chunk.Size;
                    yy = Mathf.FloorToInt(yy / (float)Chunk.Size) * Chunk.Size;

                    if (ClientchunkMap.ContainsKey(new Vector2(xx, yy)) == false)
                    {
                        Game.GameManager.GenerateChunkNet(xPos + i, yPos + y);
                        ClientchunkMap.Add(new Vector2(xx, yy), true);
                    }
                }
            }
        }
    }

    void NetFindDeleteChunk()
    {
        if (Player)
        {
            List<Chunk> DeleteChuks = new List<Chunk>(chunkMap.Values);
            Queue<Chunk> deletechuks = new Queue<Chunk>();
            for (int i = 0; i < DeleteChuks.Count; i++)
            {
                float Distanc = Vector3.Distance(Player.position, DeleteChuks[i].transform.position);
                if (Distanc > DisRenderDistance * Chunk.Size)
                {
                    deletechuks.Enqueue(DeleteChuks[i]);
                }
                while (deletechuks.Count > 0)
                {
                    Chunk chuks = deletechuks.Dequeue();
                    Game.GameManager.DeleteChunkNet((int)chuks.transform.position.x, (int)chuks.transform.position.y);
                    ClientchunkMap.Remove(chuks.transform.position);
                    chunkMap.Remove(chuks.transform.position);
                    Destroy(chuks.gameObject);
                }
            }
        }
    }

    public void ClientMakeChunkAt(int x, int y, TileSave[] tile)
    {
        x = Mathf.FloorToInt(x / (float)Chunk.Size) * Chunk.Size;
        y = Mathf.FloorToInt(y / (float)Chunk.Size) * Chunk.Size;

        if (chunkMap.ContainsKey(new Vector2(x, y)) == false)
        {
            GameObject go = Instantiate(ChunkGO, new Vector3(x, y, 0), Quaternion.identity);
            chunkMap.Add(new Vector2(x, y), go.GetComponent<Chunk>());
            go.GetComponent<Chunk>().ClientSetUpChunk(tile);
        }
    }

    public TileSave[] ServerMakeChunkAt(int x, int y, long unique)
    {
        x = Mathf.FloorToInt(x / (float)Chunk.Size) * Chunk.Size;
        y = Mathf.FloorToInt(y / (float)Chunk.Size) * Chunk.Size;

        if (chunkMap.ContainsKey(new Vector2(x, y)) == false)
        {
            GameObject go = Instantiate(ChunkGO, new Vector3(x, y, 0), Quaternion.identity);

            chunkMap.Add(new Vector2(x, y), go.GetComponent<Chunk>());

            go.GetComponent<Chunk>().Players.Add(unique);

            return go.GetComponent<Chunk>().MakeChunk();
        }
        else
        {
            return chunkMap[new Vector2(x, y)].Tiles_Save.ToArray();
        }
    }

    public void NetDeleteChunk(int x, int y, long player)
    {
        if (chunkMap.ContainsKey(new Vector2(x, y)) == true && chunkMap[new Vector2(x, y)].Players.Count == 0 || chunkMap.ContainsKey(new Vector2(x, y)) == true && chunkMap[new Vector2(x, y)].Players.Count == 1 && chunkMap[new Vector2(x, y)].Players[0] == player)
        {
            Chunk chunk = chunkMap[new Vector2(x, y)];
            chunkMap[new Vector2(x, y)].Players.RemoveAt(0);
            chunkMap.Remove(new Vector2(x, y));
            Destroy(chunk.gameObject);
        }
        else if (chunkMap.ContainsKey(new Vector2(x, y)) == true)
        {
            chunkMap[new Vector2(x, y)].Players.Remove(player);
        }
    }
}

public enum WorldType
{
    Normal = 0, DemondWorld = 1, Caves = 2, Dungeons = 3, Sky = 4
}
