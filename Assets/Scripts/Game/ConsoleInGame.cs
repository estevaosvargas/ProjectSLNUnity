using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ConsoleInGame : MonoBehaviour
{
    public string ConsoleVersion = "V0.1";
    public List<string> List = new List<string>();
    public RectTransform InveRoot;
    public GameObject Canvas;
    public GameObject SlotPrefab;
    public bool IsVisible = false;
    [Header("Input Console")]
    public InputField InputConsole;

    void Awake()
    {
        Game.ConsoleInGame = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        AddInRoolGUI("DarckConsole : " + ConsoleVersion, false, new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1), 16);
        Application.logMessageReceived += HandleLog;
        AddInRoolGUI("Debug mode activated.", true, Color.yellow);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (IsVisible != true)
            {
                IsVisible = true;
                MouselockFake.ConsoleIsOpen = true;
                MouselockFake.IsLock = true;

                if (Game.MenuManager != null)
                {
                    Game.MenuManager.CloseAll();
                }

                Canvas.SetActive(true);
            }
            else
            {
                IsVisible = false;
                MouselockFake.ConsoleIsOpen = false;
                MouselockFake.IsLock = false;
                Canvas.SetActive(false);
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

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                AddInRoolGUI("<color=red>" + logString + " : " + stackTrace + "</color>", false, Color.red);
                break;
            case LogType.Assert:
                AddInRoolGUI("<color=red>" + logString + " : " + stackTrace + "</color>", false, Color.red);
                break;
            case LogType.Exception:
                AddInRoolGUI("<color=red>" + logString +  " : " + stackTrace + "</color>", false, Color.red);
                break;
            case LogType.Warning:
                AddInRoolGUI("<color=yellow>" + logString +  " : " + stackTrace + "</color>", false, Color.yellow);
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
            AddInRoolGUI("Coming Soon", true, Color.yellow);
            #endregion
        }
        else if (string.Equals(value[0], "Info", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            AddInRoolGUI("UserID : " + Game.GameManager.UserId + "| UserName : " + Game.GameManager.UserName + " | GameVersion : " + Game.GameManager.Version, true, Color.white);
            #endregion
        }
        else if (string.Equals(value[0], "AddItem", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            Game.GameManager.MyPlayer.MyInventory.Additem(Tools.GetStringInt(value[1]), Tools.GetStringInt(value[2]));
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
            Game.TimeOfDay.TimeH = float.Parse(value[1]) / 24;
            Game.TimeOfDay.LastUpdateTime();
            AddInRoolGUI("Time Set for : " + Game.TimeOfDay.TimeH, true, Color.white);
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
        else if (string.Equals(value[0], "say", StringComparison.OrdinalIgnoreCase))
        {
            if (value.Length > 2)
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

        if (List.Contains(text) == false || command == true)
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

    public void AddInRoolGUI(string text, bool command, Color text_color)
    {
        if (List.Count >= 500)
        {
            List.Clear();
        }

        if (List.Contains(text) == false || command == true)
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