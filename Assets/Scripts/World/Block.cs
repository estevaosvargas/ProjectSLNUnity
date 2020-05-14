using UnityEngine;
using System.Collections;

[System.Serializable]
public class Block
{
    public int x;
    public int y;
    public int z;

    public TypeBlock Type;
    public BiomeType TileBiome;
    public TypeVariante typeVariante;
    public Placer PLACER_DATA;
    public TakeGO typego;

    public int HP = 100;

    public int Hora;
    public int Dia = 1;
    public int Mes = 1;

    public float density;

    [System.NonSerialized]
    public bool OcupedByOther = false;
    [System.NonSerialized]
    public DataVector3 CityPoint;
    /// <summary>
    /// If AI Can Walk.
    /// </summary>
    [System.NonSerialized]
    public bool CanWalk = false;
    [System.NonSerialized]
    public bool IsServerTile = false;
    [System.NonSerialized]
    public int MaxHP = 100;
    [System.NonSerialized]
    public Vector3 CurrentChunk;
    [System.NonSerialized]
    public GameObject BlockObject;

    public Block(int _x, int _y, int _z, float _density, TypeBlock _Type)
    {
        x = _x;
        y = _y;
        z = _z;

        Type = _Type;

        density = _density;

        /*if (y <= 0)
        {
            
        }
        else
        {
            float thisHeight = VoxelStruct.GetTerrainHeight(x, z);

            // Set the value of this point in the terrainMap.
            density = (float)y - thisHeight;

            if (density <= 0.6f && density >= 0.5f)
            {
                isSurface = true;

                System.Random rand = new System.Random(World.Instance.seed + x * y * z);

                if (rand.Next(0,10) <= 3)
                {
                    HaveTree = true;
                }

                SetBlock(BlockType.Grass);
            }
            else if (density < 0.5f)
            {
                FastNoise noise = new FastNoise(0);

                noise.SetFrequency(0.2f);
                noise.SetInterp(FastNoise.Interp.Quintic);

                density = noise.GetPerlin(x, y, z);

                if (density <= 0.5f)
                {
                    SetBlock(BlockType.DirtRoad);
                }
                else
                {
                    SetBlock(BlockType.Air);
                }
            }
            else
            {
                SetBlock(BlockType.Air);
            }
        }*/
    }

    public void RemoveBlock()
    {
        SetBlock(TypeBlock.Air);
        Game.World.GetChunk(CurrentChunk).UpdateMeshChunk();
    }

    public void PlaceBlock(TypeBlock blockType)
    {
        SetBlock(blockType);
        Game.World.GetChunk(CurrentChunk).UpdateMeshChunk();
    }

    public void DamageBloco(float damage)
    {

    }

    float getHight()
    {
        FastNoise noise = new FastNoise(0);

        noise.SetFrequency(0.01f);
        noise.SetInterp(FastNoise.Interp.Quintic);

        return y - noise.GetPerlin(x, z, 0) * 20;
    }

    public Block[] GetNeighboors(bool diagonals = false)
    {
        Block[] neighbors;

        if (diagonals)
        {
            neighbors = new Block[8];

            neighbors[0] = Game.World.GetTileAt(x, y,z + 1);//cima
            neighbors[1] = Game.World.GetTileAt(x + 1, y, z);//direita
            neighbors[2] = Game.World.GetTileAt(x, y, z - 1);//baixo
            neighbors[3] = Game.World.GetTileAt(x - 1, y, z);//esquerda

            neighbors[4] = Game.World.GetTileAt(x + 1, y, z - 1);//corn baixo direita
            neighbors[5] = Game.World.GetTileAt(x - 1, y, z + 1);//corn cima esquerda
            neighbors[6] = Game.World.GetTileAt(x + 1, y, z + 1);//corn cima direita
            neighbors[7] = Game.World.GetTileAt(x - 1, y, z - 1);//corn baixo esuqerda

        }
        else
        {
            neighbors = new Block[6];

            neighbors[0] = Game.World.GetTileAt(x, y, z - 1);//Atras
            neighbors[1] = Game.World.GetTileAt(x, y, z + 1);//Frente
            neighbors[2] = Game.World.GetTileAt(x, y + 1, z);//Cima
            neighbors[3] = Game.World.GetTileAt(x, y - 1, z);//Baixo
            neighbors[4] = Game.World.GetTileAt(x - 1, y, z);//esquerda
            neighbors[5] = Game.World.GetTileAt(x + 1, y, z);//direita
        }

        return neighbors;
    }

    public void RefreshTile()
    {

    }

    public void SaveChunk()
    {

    }

    public void SetBlock(TypeBlock blockType)
    {
        Type = blockType;
    }
}
