using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorManager : MapManager
{
    public UnityStandardAssets.Utility.SmoothFollow Cam;

    private void Start()
    {
        if (Game.GameManager.SinglePlayer || Game.GameManager.MultiPlayer)
        {
            if (!Game.GameManager.Player.PlayerObj)
            {
                Game.GameManager.Player.RequestSpawnPlayer(new Vector3(0, 1, 0), World_ID);
            }
            Cam.target = Game.GameManager.Player.PlayerObj.transform;
            Game.ConsoleInGame.LoadingScreen_Hide();
        }

        Setplayer_data();
    }

    public void Setplayer_data()
    {
        if (Game.GameManager.SinglePlayer || Game.GameManager.MultiPlayer)
        {
            
        }
    }
}
