using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickConsole : MonoBehaviour
{
    public string commandname = "";

    public void Click()
    {
        Game.ConsoleInGame.OnClickButao(commandname);
    }
}
