using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingLogo : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}