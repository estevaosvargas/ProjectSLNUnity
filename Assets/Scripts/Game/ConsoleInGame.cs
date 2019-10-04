using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ConsoleInGame : MonoBehaviour
{
    public List<string> List = new List<string>();

    public RectTransform InveRoot;
    public GameObject SlotPrefab;
    public int Size = 5;
    public float Spacing = 3;

    [Header("Input Console")]
    public InputField InputConsole;

    void Awake()
    {
        Game.ConsoleInGame = this;
    }

    private void Start()
    {
        Application.logMessageReceived += HandleLog;
        AddInRoolGUI("Debug mode activated.", true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!string.IsNullOrEmpty(InputConsole.text))
            {
                AddInRoolGUI(InputConsole.text, true);
                string[] textarray = InputConsole.text.Split(" "[0]);
                InsertCommand(textarray);
                InputConsole.text = "";//Clear Input
            }
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                AddInRoolGUI("<color=red>" + logString + " : " + stackTrace + "</color>", false);
                break;
            case LogType.Assert:
                AddInRoolGUI("<color=red>" + logString + " : " + stackTrace + "</color>", false);
                break;
            case LogType.Exception:
                AddInRoolGUI("<color=red>" + logString +  " : " + stackTrace + "</color>", false);
                break;
            case LogType.Warning:
                AddInRoolGUI("<color=yellow>" + logString +  " : " + stackTrace + "</color>", false);
                break;
            case LogType.Log:
                AddInRoolGUI("<color=white>" + logString + "</color>", false);
                break;
            default:
                
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
            AddInRoolGUI("Coming Soon", true);
            #endregion
        }
        else if (string.Equals(value[0], "savegame", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            AddInRoolGUI("Coming Soon", true);
            #endregion
        }
        else if (string.Equals(value[0], "Connect", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            AddInRoolGUI("Coming Soon", true);
            #endregion
        }
        else if (string.Equals(value[0], "Info", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            AddInRoolGUI("UserID : " + Game.GameManager.UserId + "| UserName : " + Game.GameManager.UserName + " | GameVersion : " + Game.GameManager.Version, true);
            #endregion
        }
        else if (string.Equals(value[0], "AddItem", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            Game.GameManager.MyPlayer.MyInventory.Additem(Tools.GetStringInt(value[1]), Tools.GetStringInt(value[2]));
            AddInRoolGUI("Add This Item on your inventory : " + ItemManager.Instance.GetItem(Tools.GetStringInt(value[1])).Name, true);
            #endregion
        }
        else if (string.Equals(value[0], "Help", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            AddInRoolGUI("Coming Soon", true);
            #endregion
        }
        else if (string.Equals(value[0], "SetTime", StringComparison.OrdinalIgnoreCase))
        {
            #region Comand
            Game.TimeOfDay.TimeH = float.Parse(value[1]) / 24;
            Game.TimeOfDay.LastUpdateTime();
            AddInRoolGUI("Time Set for : " + Game.TimeOfDay.TimeH, true);
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
            AddInRoolGUI("Max_Fps set to : " + Application.targetFrameRate, true);
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
                AddInRoolGUI("You Say : " + valuefinal, true);
                Game.MenuManager.PopUpName(valuefinal);
            }
            else
            {
                Game.MenuManager.PopUpName(value[1]);
            }
        }
        else
        {
            AddInRoolGUI("Don't have this command : " + value[0], true);
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

    public void AddInRoolGUI(string text, bool command)
    {
        if (Application.isLoadingLevel)
        {
            return;
        }

        if (List.Count >= 500)
        {
            List.Clear();
        }

        if (List.Contains(text) == false || command == true)
        {
            ClearCanvas();
            List.Add(text);

            foreach (string strig in List)
            {
                GameObject newAnimal = Instantiate(SlotPrefab) as GameObject;
                newAnimal.GetComponent<Text>().text = strig;

                newAnimal.transform.SetParent(InveRoot.gameObject.transform);
                newAnimal.transform.localScale = Vector3.one;
            }
        }
    }
}