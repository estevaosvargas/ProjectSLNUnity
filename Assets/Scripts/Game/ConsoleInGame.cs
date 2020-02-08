using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;

public class ConsoleInGame : MonoBehaviour
{
    public string ConsoleVersion = "V0.1";
    public List<string> List = new List<string>();
    public RectTransform InveRoot;
    public RectTransform HelpRoot;
    public GameObject ConsoleWindow;
    public GameObject SlotPrefab;
    public GameObject HelpPrefab;
    public bool IsVisible = false;
    [Header("Input Console")]
    public InputField InputConsole;
    [Header("LoadingScreen")]
    public bool IsLoading = false;
    public GameObject LoadingscreenObj;
    public Text LoadingText;

    public bool Collapse = true;

    List<string> CommandsData = new List<string>();

    void Awake()
    {
        Game.ConsoleInGame = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadingScreen_Hide();
        AddInRoolGUI("DarckConsole : " + ConsoleVersion, false, new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1), 16);
        Application.logMessageReceived += HandleLog;
        AddInRoolGUI("Debug mode activated.", true, Color.yellow);
        LoadCommands();
    }

    private void LoadCommands()
    {
        AddInRoolGUI("Commands Loading...", true, Color.yellow);

        CommandsData.Add("disconnect");
        CommandsData.Add("connect");
        CommandsData.Add("settime");
        CommandsData.Add("additem");
        CommandsData.Add("clear");
        CommandsData.Add("info");
        CommandsData.Add("fpsmax");
        CommandsData.Add("help");
        CommandsData.Add("say");
        CommandsData.Add("quit");
        CommandsData.Add("console.collapse");
        CommandsData.Add("loadingscreen.show");
        CommandsData.Add("discordmsg");

        AddInRoolGUI("Commands Loaded", true, Color.yellow);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.Joystick1Button6))
        {
            if (IsVisible != true)
            {
                IsVisible = true;
                MouselockFake.ConsoleIsOpen = true;
                MouselockFake.LockUnlock(true);

                if (Game.MenuManager != null)
                {
                    Game.MenuManager.CloseAll();
                }
                ConsoleWindow.SetActive(true);

                InputConsole.ActivateInputField();
            }
            else
            {
                IsVisible = false;
                MouselockFake.ConsoleIsOpen = false;
                MouselockFake.LockUnlock(false);
                ConsoleWindow.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!string.IsNullOrEmpty(InputConsole.text))
            {
                AddInRoolGUI(InputConsole.text, true, Color.white);
                string[] textarray = InputConsole.text.Split(" "[0]);
                InsertCommand(textarray);
                InputConsole.text = "";//Clear Input
                InputConsole.ActivateInputField();
            }
        }        
    }

    public void OnClickButao(string text_to_input)
    {
        //InputConsole.ActivateInputField();
        InputConsole.text = text_to_input;
        InputConsole.Select();
        foreach (Transform child in HelpRoot.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnInputChange()
    {
        if (!string.IsNullOrEmpty(InputConsole.text))
        {
            string[] commands = ProcessHelpCommand(InputConsole.text);

            foreach (Transform child in HelpRoot.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var item in commands)
            {
                if (item != null)
                {
                    GameObject newAnimal = Instantiate(HelpPrefab) as GameObject;
                    newAnimal.transform.SetParent(HelpRoot.gameObject.transform);
                    newAnimal.transform.localScale = Vector3.one;
                    newAnimal.GetComponent<ButtonClickConsole>().commandname = item;
                    newAnimal.GetComponentInChildren<Text>().text = item;
                }
            }
        }
        else
        {
            foreach (Transform child in HelpRoot.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                AddInRoolGUI("<color=red>" + logString + " : </color>" + stackTrace, false, Color.white);
                break;
            case LogType.Assert:
                AddInRoolGUI("<color=red>" + logString + " : </color>" + stackTrace, false, Color.white);
                break;
            case LogType.Exception:
                AddInRoolGUI("<color=red>" + logString + " : </color>" + stackTrace, false, Color.white);
                break;
            case LogType.Warning:
                AddInRoolGUI("<color=yellow>" + logString + " : </color>" + stackTrace, false, Color.white);
                break;
            case LogType.Log:
                AddInRoolGUI("<color=white>" + logString + "</color>", false, Color.white);
                break;
            default:
                AddInRoolGUI("<color=white>" + logString + "</color>", false, Color.white);
                break;
        }
    }

    public void InsertCommand(string[] value)
    {
        //value [0] is the command, [1]>>>> is the values
        //<color=red>Your Text With Color Hear</color> // To Add Color of Part Of the Text

        if (string.Equals(value[0], "Quit", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            DarckNet.Network.Disconnect();
            Application.Quit();
            #endregion
        }
        else if (string.Equals(value[0], "loadsave", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            AddInRoolGUI("Coming Soon", true, Color.yellow);
            #endregion
        }
        else if (string.Equals(value[0], "savegame", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            AddInRoolGUI("Coming Soon", true, Color.yellow);
            #endregion
        }
        else if (string.Equals(value[0], "Connect", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand

            if (value.Length == 1)
            {
                if (DarckNet.Network.Connect("127.0.0.1", 25000, "") == null) { return; }
                Debug.LogWarning("Connecting in local host, if you want connect in specifique server, use like this. EX: Connect 127.0.0.1 25000 ServerPassword");
            }
            else if (value.Length == 2)
            {
                if (DarckNet.Network.Connect(value[1], 25000, "") == null) { return; }
            }
            else if (value.Length == 3)
            {
                if (DarckNet.Network.Connect(value[1], int.Parse(value[2]), "") == null) { return; }
            }
            else if (value.Length == 4)
            {
                if (DarckNet.Network.Connect(value[1], int.Parse(value[2]), value[3]) == null) { return; }
            }
            
            Game.GameManager.SinglePlayer = false;
            Game.GameManager.MultiPlayer = true;
            GameManager.Playing = true;

            AddInRoolGUI("Coming Soon", true, Color.yellow);
            #endregion
        }
        else if (string.Equals(value[0], "Info", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            AddInRoolGUI("UserID : " + Game.GameManager.Player.UserID + "| UserName : " + Game.GameManager.Player.UserName + " | GameVersion : " + Game.GameManager.Version, true, Color.white);
            #endregion
        }
        else if (string.Equals(value[0], "AddItem", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            Game.GameManager.Player.PlayerObj.Inve.Additem(Tools.GetStringInt(value[1]), Tools.GetStringInt(value[2]));
            AddInRoolGUI("Add This Item on your inventory : " + ItemManager.Instance.GetItem(Tools.GetStringInt(value[1])).Name, true, Color.white);
            #endregion
        }
        else if (string.Equals(value[0], "Help", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            AddInRoolGUI("Coming Soon", true, Color.yellow);
            #endregion
        }
        else if (string.Equals(value[0], "SetTime", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            if (value.Length == 2)
            {
                Game.TimeOfDay.timeOfDay = float.Parse(value[1]);
                Game.TimeOfDay.LastUpdateTime();
                AddInRoolGUI("Time Set for : " + Game.TimeOfDay.timeOfDay, true, Color.white);
            }
            else
            {
                AddInRoolGUI("Sorry you cant set time with-out value!", false, Color.red);
            }
            #endregion
        }
        else if (string.Equals(value[0], "Clear", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            ClearConsole();
            #endregion
        }
        else if (string.Equals(value[0], "fpsmax", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            Application.targetFrameRate = int.Parse(value[1]);
            AddInRoolGUI("Max_Fps set to : " + Application.targetFrameRate, true, Color.white);
            #endregion
        }
        else if (string.Equals(value[0], "disconnect", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            DarckNet.Network.Disconnect();

            Game.GameManager.ClearObjects();
            Game.GameManager.SinglePlayer = false;
            Game.GameManager.MultiPlayer = false;
            GameManager.Playing = false;

            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            #endregion
        }
        else if (string.Equals(value[0], "console.collapse", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            Collapse = bool.Parse(value[1]);
            AddInRoolGUI("Console Collapse : " + Collapse.ToString(), true, Color.white);
            #endregion
        }
        else if (string.Equals(value[0], "loadingscreen.show", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            LoadingScreen_Show();
            AddInRoolGUI("LoadingScreen Is Showing!!", true, Color.white);
            #endregion
        }
        else if (string.Equals(value[0], "discordmsg", StringComparison.OrdinalIgnoreCase))
        {
            if (value.Length >= 2)
            {
                string valuefinal = "";
                for (int i = 0; i < value.Length; i++)
                {
                    if (i != 0)
                    {
                        valuefinal += value[i] + " ";
                    }
                }
                StartCoroutine(TestApi("log","(" + Game.GameManager.Player.UserID + ") Say: " + valuefinal));
                Game.MenuManager.PopUpName("(" + Game.GameManager.Player.UserID + ") Say: " + valuefinal);
            }
            else
            {
                Game.MenuManager.PopUpName(value[1]);
            }
        }
        else if (string.Equals(value[0], "say", StringComparison.OrdinalIgnoreCase))
        {
            if (value.Length >= 2)
            {
                string valuefinal = "";
                for (int i = 0; i < value.Length; i++)
                {
                    if (i != 0)
                    {
                        valuefinal += value[i] + " ";
                    }
                }
                AddInRoolGUI("You Say : " + valuefinal, true, Color.white);
                Game.MenuManager.PopUpName(valuefinal);
            }
            else
            {
                Game.MenuManager.PopUpName(value[1]);
            }
        }
        else
        {
            AddInRoolGUI("Don't have this command : " + value[0], true, Color.red);
        }
    }

    public void ClearCanvas()
    {
        if (InveRoot != null)
        {
            foreach (Transform child in InveRoot.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    void ClearConsole()
    {
        List.Clear();
        ClearCanvas();
    }

    public void AddInRoolGUI(string text, bool command, Color text_color, int size)
    {
        if (List.Count >= 500)
        {
            List.Clear();
        }

        if (List.Contains(text) == false || command == true || Collapse == false)
        {
            if (InveRoot)
            {
                List.Add(text);

                GameObject newAnimal = Instantiate(SlotPrefab) as GameObject;

                Rect rec = newAnimal.GetComponent<RectTransform>().rect;

                newAnimal.GetComponent<Text>().text = text;
                newAnimal.GetComponent<Text>().color = text_color;
                newAnimal.GetComponent<Text>().fontSize = size;
                newAnimal.transform.SetParent(InveRoot.gameObject.transform);
                newAnimal.transform.localScale = Vector3.one;
                newAnimal.GetComponent<RectTransform>().sizeDelta = new Vector2(newAnimal.GetComponent<RectTransform>().sizeDelta.x, 14 + size);
            }
        }
    }

    public void AddInRoolGUI(string text, bool command, Color text_color)
    {
        if (List.Count >= 500)
        {
            List.Clear();
        }

        if (List.Contains(text) == false || command == true || Collapse == false)
        {
            if (InveRoot)
            {
                List.Add(text);
                GameObject newAnimal = Instantiate(SlotPrefab) as GameObject;
                newAnimal.GetComponent<Text>().text = text;
                newAnimal.GetComponent<Text>().color = text_color;
                newAnimal.transform.SetParent(InveRoot.gameObject.transform);
                newAnimal.transform.localScale = Vector3.one;
            }
        }
    }

    public string[] ProcessHelpCommand(string Command)
    {
        string[] commands = GetCommands(Command);

        for (int i =0; i < commands.Length; i++)
        {
            if (string.Equals(commands[i], "connect", StringComparison.OrdinalIgnoreCase))
            {
                commands[i] += " 127.0.0.1 2500 ";
            }
            else if(string.Equals(commands[i], "additem", StringComparison.OrdinalIgnoreCase))
            {
                commands[i] += " 0 50";
            }
            else if (string.Equals(commands[i], "say", StringComparison.OrdinalIgnoreCase))
            {
                commands[i] += " Hello World!";
            }
            else if (string.Equals(commands[i], "fpsmax", StringComparison.OrdinalIgnoreCase))
            {
                commands[i] += " 60";
            }
            else if (string.Equals(commands[i], "settime", StringComparison.OrdinalIgnoreCase))
            {
                commands[i] += " 12";
            }
            else if (string.Equals(commands[i], "console.collapse", StringComparison.OrdinalIgnoreCase))
            {
                commands[i] += " true";
            }
            else if (string.Equals(commands[i], "discordmsg", StringComparison.OrdinalIgnoreCase))
            {
                commands[i] += " Hello from console in game to discord (:";
            }
        }

        return commands;
    }

    public string[] GetCommands(string command)
    {
        List<string> commandsfound = new List<string>();

        command = command.ToLower();

        foreach (var item in CommandsData)
        {
            char[] characters = command.ToCharArray();
            char[] COMMAND_NAME = item.ToCharArray();
            string letterdound = "";

            for (int i =0; i < characters.Length; i++)
            {
                letterdound += characters[i].ToString();
            }

            if (item.Contains(letterdound))
            {
                if (!commandsfound.Contains(item))
                {
                    commandsfound.Add(item);
                }
            }

            if (string.Equals(letterdound.ToString(), item.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                if (!commandsfound.Contains(item))
                {
                    commandsfound.Add(item);
                }
            }
        }
        return commandsfound.ToArray();
    }

    #region LoadingScreen
    public void LoadingScreen_Show()
    {
        LoadingscreenObj.SetActive(true);
    }

    public void LoadingScreen_Hide()
    {
        LoadingscreenObj.SetActive(false);
    }

    public void UpdateLoadingText(string text)
    {
        LoadingText.text = text;
    }
    #endregion

    #region HttpApi
    IEnumerator TestApi(string type, string msg)
    {
        using (WWW www = new WWW("https://darckcomsoftdb.000webhostapp.com/api/discord?type=" + type + "&msg=" + msg))
        {
            yield return www;
        }
    }
    #endregion
}

public class CommandHelp
{
    public string COMMAND_NAME;
    public CommandType type;

    public CommandHelp(string namec, CommandType typec)
    {
        COMMAND_NAME = namec;
        type = typec;
    }
}

public enum CommandType
{
    none, quit, connect, settime
}