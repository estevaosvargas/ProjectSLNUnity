using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharItemGUI : MonoBehaviour
{
    public CharListGUI GUI;
    public int ID = -1;
    public Text NAME;
    public Text WorldName;

    public void SetUp(int id, string name_char,string world_name, CharListGUI Gui)
    {
        ID = id;
        NAME.text = name_char;
        if (world_name != "")
        {
            WorldName.text = "World: " + world_name;
        }
        GUI = Gui;
    }

    public void PlayWithThis()
    {
        GUI.PlayWithPlayer(ID);
    }

    public void DeleteThis()
    {
        GUI.DeleteOne(ID);
    }

    public void CreateNew()
    {
        GUI.CharCreatorWindow.SetActive(true);
        GUI.CharListWindow.SetActive(false);
    }
}
