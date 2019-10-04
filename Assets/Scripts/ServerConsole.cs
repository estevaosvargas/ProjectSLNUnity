using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerConsole : MonoBehaviour
{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

    Windows.ConsoleWindow console = new Windows.ConsoleWindow();
    Windows.ConsoleInput input = new Windows.ConsoleInput();


    string strInput;

    //
    // Create console window, register callbacks
    //
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        console.Initialize();
        console.SetTitle("2DTopdown - Server");

        input.OnInputText += OnInputText;

        //Application.RegisterLogCallback(HandleLog);
 
        Debug.Log("Console Started");
    }

    void Start()
    {
        Application.logMessageReceived += HandleLog;
    }

    //
    // Text has been entered into the console
    // Run it as a console command
    //
    void OnInputText(string obj)
    {
        string[] textarray = obj.Split(" "[0]);

        if (string.Equals(textarray[0], "SetTime", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            Game.TimeOfDay.TimeH = float.Parse(textarray[1]) / 24;
            Game.TimeOfDay.LastUpdateTime();
            Debug.Log("Time Set for : " + Game.TimeOfDay.TimeH);
            #endregion
        }
        else if (string.Equals(textarray[0], "Stop", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            Debug.Log("Server is Shutingdown...");
            DarckNet.Network.Disconnect();
            Debug.Log("Network Is Finished...");
            Debug.Log("Saving...");
            //Save Method soon here
            System.Threading.Thread.Sleep(100);
            Debug.Log("Quiting...");
            Application.Quit();
            #endregion
        }
    }

    //
    // Debug.Log* callback
    //
    void HandleLog(string message, string stackTrace, LogType type)
    {
        System.Console.WriteLine(message);
        System.Console.ForegroundColor = System.ConsoleColor.White;
    }

    //
    // Update the input every frame
    // This gets new key input and calls the OnInputText callback
    //
    void Update()
    {
        input.Update();
    }

    //
    // It's important to call console.ShutDown in OnDestroy
    // because compiling will error out in the editor if you don't
    // because we redirected output. This sets it back to normal.
    //
    void OnDestroy()
    {
        console.Shutdown();
    }

#endif
}