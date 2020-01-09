using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainMenu : Menus
{
    public RawImage BackGround;
    public Text Version;

    void Start()
    {
        Version.text = "©2020 - Darckcomsoft. " + Game.GameManager.Version;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}