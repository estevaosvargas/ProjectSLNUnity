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
        Version.text = "©2019 Darckcomsoft. | Unity("+Application.unityVersion + "), " + "Plataform (" + SystemInfo.operatingSystem + "), " + Application.productName + " - Version(" + Game.GameManager.Version + ")";
    }

    #region Easter
    int etgg = 0;
    public Text etgtext;
    public Text etgtext2;
    #endregion

    void Update()
    {
        #region Easter
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            etgg += 1;

            if (etgg == 3)
            {
                etgtext.text = "666";
                etgtext2.text = "@666 Darckcomsoft : You Found Easter egg, don't say nothing, sheee!";
                etgtext.color = Color.red;
                etgtext2.color = Color.red;

                Color color = new Color();

                ColorUtility.TryParseHtmlString("#872A2A00", out color);

                Camera.main.backgroundColor = color;
            }
        }
        #endregion
    }


    public void ExitGame()
    {
        Application.Quit();
    }
}