using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingLogo : MonoBehaviour
{
    public GameObject Console;

    void Start()
    {
        if (!Game.ConsoleInGame)
        {
            Instantiate(Console);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}