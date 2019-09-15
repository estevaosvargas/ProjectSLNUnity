using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menus : MonoBehaviour
{
    public List<MenuLoader> MenuList = new List<MenuLoader>();//list of all menus

    public MenuLoader GetMenuByName(string name)//Get menu by name, you can acsses menu by this
    {
        foreach (var item in MenuList)
        {
            if (item.Name == name)
            {
                return item;
            }
        }

        Debug.LogError("Don't have this menu : " + name);
        return null;
    }

    public bool CheckifEnable(string name)//Check if specifique menu by name are activated
    {
        foreach (var item in MenuList)
        {
            if (item.Name == name)
            {
                return item.gameObject.activeInHierarchy;
            }
        }

        Debug.LogError("Don't have this menu : " + name);
        return false;
    }

    public GameObject OpenMenuName(string name)//Open Menu By Name, and close all others menus
    {
        MenuLoader have = null;

        foreach (var item in MenuList)
        {
            if (item.gameObject.activeInHierarchy == true)
            {
                item.gameObject.SetActive(false);
            }

            if (item.Name == name)
            {
                have = item;
            }
        }

        if (have != null)
        {
            have.gameObject.SetActive(true);
            return have.gameObject;
        }

        Debug.LogError("Don't have this menu : " + name);
        return null;
    }

    public GameObject OpenMenuNameNoClose(string name)//Open Menu By Name
    {
        MenuLoader have = null;

        foreach (var item in MenuList)
        {
            if (item.Name == name)
            {
                have = item;
            }
        }

        if (have != null)
        {
            have.gameObject.SetActive(true);
            return have.gameObject;
        }

        Debug.LogError("Don't have this menu : " + name);
        return null;
    }


    public void OpenMenuName(string name, string tovoid, params object[] parms)//Open menu by name, whit paramter, you can send data to the menu
    {
        MenuLoader have = null;

        foreach (var item in MenuList)
        {
            if (item.gameObject.activeInHierarchy == true)
            {
                item.gameObject.SetActive(false);
            }

            if (item.Name == name)
            {
                have = item;
            }
        }

        if (have != null)
        {
            have.gameObject.SetActive(true);
            have.SendMessage(tovoid, parms, SendMessageOptions.DontRequireReceiver);
            return;
        }

        Debug.LogError("Don't have this menu : " + name);
    }

    public void CloseMenuName(string name)//Close menu by name
    {
        foreach (var item in MenuList)
        {
            if (item.Name == name)
            {
                item.gameObject.SetActive(false);
                return;
            }
        }
        Debug.LogError("Don't have this menu : " + name);
    }
}

public class MenuManager : Menus
{
    public GameObject Canavas;
    public InventoryGUI InveGui;

    public GameObject popname;
    public GameObject popup;

    public BarIcons LifeBar;
    public BarIcons AguaBar;
    public BarIcons FoodBar;
    public BarIcons ManaBar;

    void Start()
    {
        Game.MenuManager = this;
        Canavas.SetActive(GameManager.Playing);
    }

    void Update()
    {
        InputMenu();
    }

    void InputMenu()
    {
        if (Game.GameManager.MyPlayer.MyPlayerMove)
        {
            if (Game.GameManager.MyPlayer.MyPlayerMove.IsAlive)
            {
                if (Input.GetKeyDown(KeyCode.BackQuote))
                {
                    //open DevConsole
                    if (CheckifEnable("DevConsole") == true)
                    {
                        CloseMenuName("DevConsole");
                        MouselockFake.IsLock = false;
                    }
                    else
                    {
                        OpenMenuName("DevConsole");
                        MouselockFake.IsLock = true;
                    }
                }

                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.I))
                {
                    if (CheckifEnable("DevConsole") == false)
                    {
                        if (CheckifEnable("Inventory") == true)
                        {
                            CloseMenuName("Inventory");
                            MouselockFake.IsLock = false;
                        }
                        else
                        {
                            OpenMenuNameNoClose("Inventory");
                            InveGui.OpenInev(Game.GameManager.MyPlayer.MyInventory);
                            MouselockFake.IsLock = true;
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.F1))
                {
                    //open info menu
                    if (CheckifEnable("DebugInfo") == true)
                    {
                        CloseMenuName("DebugInfo");
                    }
                    else
                    {
                        OpenMenuName("DebugInfo");
                    }
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (CheckifEnable("InGameMenu") == true)
                    {
                        CloseMenuName("InGameMenu");
                        MouselockFake.IsLock = false;
                    }
                    else
                    {
                        OpenMenuName("InGameMenu");
                        MouselockFake.IsLock = true;
                    }
                }

                //Player StatusMenu
                if (GameInput.STATUSButtonDown())
                {
                    if (CheckifEnable("Status") == true)
                    {
                        CloseMenuName("Status");
                        MouselockFake.IsLock = false;
                    }
                    else
                    {
                        OpenMenuName("Status");
                        MouselockFake.IsLock = true;
                    }
                }
            }
            else
            {
                //You can do some input code after player is dead
            }
        }
    }

    public void OpenRespawn()
    {
        OpenMenuName("Respawn");
        MouselockFake.IsLock = true;
    }

    public void Respawn()
    {
        WorldManager.This.SetUpPlayer(0,0);

        DCallBack.Call(CallType.OnRespawn);

        CloseMenuName("Respawn");
        MouselockFake.IsLock = false;
    }

    public void RespawnBed()
    {
        WorldManager.This.SetUpPlayer(0, 0);

        DCallBack.Call(CallType.OnRespawn);

        CloseMenuName("Respawn");
        MouselockFake.IsLock = false;
    }

    public void OpenInveContainer(Inventory cont)
    {
        if (CheckifEnable("Inventory") == true)
        {
            CloseMenuName("Inventory");
            MouselockFake.IsLock = false;
        }
        else
        {
            OpenMenuName("Inventory");
            InveGui.OpenInevContainer(Game.GameManager.MyPlayer.MyInventory, cont);
            MouselockFake.IsLock = true;
        }

        if (CheckifEnable("Inventory") == true || CheckifEnable("InGameMenu") == true || CheckifEnable("DevConsole") == true || CheckifEnable("Status") == true)
        {
            MouselockFake.IsLock = true;
        }
        else
        {
            MouselockFake.IsLock = false;
        }
    }

    public void ExitApplication()
    {
        GameManager.Playing = false;
        Application.Quit();
    }

    public void ExitToMenu()
    {
        DarckNet.Network.Disconnect();

        Game.GameManager.ClearObjects();
        Game.GameManager.SinglePlayer = false;
        Game.GameManager.MultiPlayer = false;
        GameManager.Playing = false;

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void Resume()
    {
        CloseMenuName("InGameMenu");
        MouselockFake.IsLock = false;
    }

    public void Options()
    {

    }

    public void PopUpName(string name)
    {
        GameObject obj = Instantiate(popname, Canavas.transform.position, Quaternion.identity);
        obj.GetComponentInChildren<Text>().text = name;

        RectTransform rect = obj.GetComponent<RectTransform>();

        rect.anchoredPosition = new Vector2(0, 0);
        rect.localScale = new Vector3(1, 1, 1);

        Destroy(obj, 5);
    }

    public void PopUp(string name)
    {
        GameObject obj = Instantiate(popup, Canavas.transform.position, Quaternion.identity);
        obj.GetComponentInChildren<Text>().text = name;

        RectTransform rect = obj.GetComponent<RectTransform>();

        rect.anchoredPosition = new Vector2(0, 0);
        rect.localScale = new Vector3(1, 1, 1);

        Destroy(obj, 5);
    }
}