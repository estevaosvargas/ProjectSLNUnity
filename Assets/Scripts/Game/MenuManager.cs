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

    public void CloseAll()//Close menu by name
    {
        foreach (var item in MenuList)
        {
            item.gameObject.SetActive(false);
        }
    }
}

public class MenuManager : Menus
{
    public GameObject Canavas;
    public InventoryGUI InveGui;

    public GameObject popname;
    public GameObject popup;

    public BarIcons LifeBar;
    public BarIcons EnergyBar;
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
        if (Game.GameManager.Player.PlayerObj)
        {
            if (Game.GameManager.Player.PlayerObj.IsAlive)
            {
                if (!MouselockFake.ConsoleIsOpen)
                {
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Joystick1Button3))
                    {
                        if (CheckifEnable("Inventory") == true)
                        {
                            CloseMenuName("Inventory");
                            InveGui.CloseInve(Game.GameManager.Player.PlayerObj.Inve);
                            MouselockFake.LockUnlock(false);
                        }
                        else
                        {
                            OpenMenuNameNoClose("Inventory");
                            InveGui.OpenInev(Game.GameManager.Player.PlayerObj.Inve);
                            MouselockFake.LockUnlock(true);
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
                    {
                        if (CheckifEnable("InGameMenu") == true)
                        {
                            CloseMenuName("InGameMenu");
                            MouselockFake.LockUnlock(false);
                        }
                        else
                        {
                            OpenMenuName("InGameMenu");
                            MouselockFake.LockUnlock(true);
                        }
                    }

                    //Player StatusMenu
                    if (GameInput.STATUSButtonDown() || Input.GetKeyDown(KeyCode.Joystick1Button2))
                    {
                        if (CheckifEnable("Status") == true)
                        {
                            CloseMenuName("Status");
                            MouselockFake.LockUnlock(false);
                        }
                        else
                        {
                            OpenMenuName("Status");
                            MouselockFake.LockUnlock(true);
                        }
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
        MouselockFake.LockUnlock(true);
    }

    public void Respawn()
    {
        if (Game.GameManager.SinglePlayer || Game.GameManager.MultiPlayer)
        {
            Game.GameManager.Player.RequestSpawnPlayer(new Vector3(UnityEngine.Random.Range(-100, 100), 0, UnityEngine.Random.Range(-100, 100)), Game.WorldGenerator.World_ID);

            DCallBack.Call(CallType.OnRespawn);
            CloseMenuName("Respawn");
            MouselockFake.LockUnlock(false);
        }
    }

    public void RespawnBed()
    {
        if (Game.GameManager.SinglePlayer || Game.GameManager.MultiPlayer)
        {
            Game.GameManager.Player.RequestSpawnPlayer(new Vector3(UnityEngine.Random.Range(-100, 100), 0, UnityEngine.Random.Range(-100, 100)), Game.WorldGenerator.World_ID);

            DCallBack.Call(CallType.OnRespawn);
            CloseMenuName("Respawn");
            MouselockFake.LockUnlock(false);
        }
    }

    public void OpenInveContainer(Inventory cont)
    {
        if (CheckifEnable("Inventory") == true)
        {
            CloseMenuName("Inventory");
            InveGui.CloseInve(cont);
            MouselockFake.LockUnlock(false);
        }
        else
        {
            OpenMenuName("Inventory");
            InveGui.OpenInevContainer(Game.GameManager.Player.PlayerObj.Inve, cont);
            MouselockFake.LockUnlock(true);
        }

        if (CheckifEnable("Inventory") == true || CheckifEnable("InGameMenu") == true || CheckifEnable("Status") == true)
        {
            MouselockFake.LockUnlock(true);
        }
        else
        {
            MouselockFake.LockUnlock(false);
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
        MouselockFake.LockUnlock(false);
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