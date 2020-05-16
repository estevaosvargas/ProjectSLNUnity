using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All option and methods for maps
/// </summary>
public class MapManager : DarckNet.DarckMonoBehaviour
{
    public int World_ID = 0;
    public WorldType worldType;
    public World World;
    public bool haveSpawnPoint = false;
    public Transform SpawnPoint;
    public int spawnRange = 1000;
    public bool readyPlayer = false;
    public GameObject Sun;

    private void Awake()
    {
        Game.MapManager = this;
    }

    void Start()
    {
        World = GetComponent<World>();

        SpawnPlayerFirst();//SpawnPlayer

        if (DarckNet.Network.IsServer)
        {
            DarckNet.Network.Instantiate(Sun, Vector3.zero, Quaternion.identity, World_ID);
            Debug.Log("SERVER: Sun Spawned");
        }
    }

    private void SpawnPlayerFirst()
    {
        Vector3 spawnPoint = Vector3.zero;
        if (SpawnPoint != null)
        {
            spawnPoint = SpawnPoint.position;
        }
        else
        {
            spawnPoint = Biome.GetBiomeYPosition(Random.Range(0, spawnRange), 0, Random.Range(0, spawnRange));//this is gone change for a Biome.getYposition
        }

        Game.GameManager.Player.RequestSpawnPlayer(spawnPoint, World_ID);
    }

    public void SpawnPlayer(Vector3 position)
    {
        PlayerSpawn();
        Debug.Log("You request to spawn in a bed");
        Game.GameManager.Player.RequestSpawnPlayer(position, World_ID);
    }

    public void PlayerSpawn()
    {
        Game.MenuManager.CloseMenuName("Respawn");
        MouselockFake.LockUnlock(false);
    }

    public void PlayerDead(EntityPlayer player)
    {
        readyPlayer = false;
        Game.MenuManager.OpenRespawn();
    }

    /// <summary>
    /// If this map is finihed loading
    /// </summary>
    public void FinishedLoading()
    {
        readyPlayer = false;
        Game.ConsoleInGame.LoadingScreen_Hide();
    }

    public void PlayerFinishedLoad()
    {
        switch (worldType)
        {
            case WorldType.Procedural:
                World.StartWorld();
                break;
            case WorldType.Plain:
                World.StartWorld();
                break;
            case WorldType.Dev:
                FinishedLoading();
                break;
            case WorldType.DevProcedural:
                FinishedLoading();
                break;
        }
    }
}
