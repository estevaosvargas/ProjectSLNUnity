using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerBuild : MonoBehaviour
{
    public BuildType Type;
    public string OwnerName = "";
    public string OwnerId = "";
    public int pop = 0;
    public int maxpop = 0;

    void Start()
    {

    }

    void Update()
    {

    }
}

public enum BuildType
{
    none, House, Market, Port, Wall, WatchTwoer, AttkTwoer, Smith, Igreja, Padeiro, Lenhador, BlackMarket, WaterBuild, Açogeuiro, Celeiro
}