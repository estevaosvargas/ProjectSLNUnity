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
        Application.logMessageReceived += HandleLog;

        Debug.Log("Console Started");
    }

    //
    // Text has been entered into the console
    // Run it as a console command
    //
    void OnInputText(string obj)
    {
        
    }

    //
    // Debug.Log* callback
    //
    void HandleLog(string message, string stackTrace, LogType type)
    {
        if (message == "This uLink evaluation license is temporary. You will need to purchase a full license at muchdifferent.com/ulink. If you've bought uLink from the Asset Store, please contact sales@muchdifferent.com to get your license key.")
        {

        }else
        {
            System.Console.WriteLine(message);
            System.Console.ForegroundColor = System.ConsoleColor.White;
            //input.RedrawInputLine();
        }
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