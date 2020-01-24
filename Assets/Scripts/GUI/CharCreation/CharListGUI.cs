using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CharListGUI : MonoBehaviour
{
    public GameObject CharItem;
    public GameObject PrefabCreate;
    public GameObject Root;
    public List<CharacterLista> characterDatas = new List<CharacterLista>();
    public List<WorldList> List = new List<WorldList>();
    [Header("CharCreatorWindow")]
    public InputField Input_UserName;
    public InputField Input_WorldName;
    public InputField Input_SeedName;
    public Button Button_CharCreate;
    public Sprite CharCurretn;
    public RawImage CharPreview;
    [Header("OpenClose")]
    public GameObject CharCreatorWindow;
    public GameObject CharListWindow;

    void Start()
    {
        Debug.Log("DarckcomCharCreator - V0.3");
        characterDatas = new List<CharacterLista>(SaveWorld.LoadChars());

        if (File.Exists(Path.GetFullPath("Saves./SavesData.data")))
        {
            foreach (WorldList obj in LoadInfo())
            {
                List.Add(obj);
            }
        }
    }

    public void RenderList()
    {
        ClearCanvas();

        if (characterDatas.Count == 0)
        {
            GameObject newcahrobj = Instantiate(PrefabCreate) as GameObject;
            newcahrobj.transform.SetParent(Root.transform);
            newcahrobj.transform.localScale = Vector3.one;
            newcahrobj.GetComponent<CharItemGUI>().SetUp(-1, "Create New Character", "", this);
        }

        for (int i = 0; i < characterDatas.Count; i++)
        {
            if (i == characterDatas.Count -1)
            {
                GameObject newAnimal = Instantiate(CharItem) as GameObject;
                newAnimal.transform.SetParent(Root.transform);
                newAnimal.transform.localScale = Vector3.one;
                newAnimal.GetComponent<CharItemGUI>().SetUp(characterDatas[i].ID, characterDatas[i].NAME, characterDatas[i].WorldName, this);

                if (i < 7)
                {
                    GameObject newcahrobj = Instantiate(PrefabCreate) as GameObject;
                    newcahrobj.transform.SetParent(Root.transform);
                    newcahrobj.transform.localScale = Vector3.one;
                    newcahrobj.GetComponent<CharItemGUI>().SetUp(-1, "Create New Character", "", this);
                }
            }
            else
            {
                GameObject newAnimal = Instantiate(CharItem) as GameObject;
                newAnimal.transform.SetParent(Root.transform);
                newAnimal.transform.localScale = Vector3.one;
                newAnimal.GetComponent<CharItemGUI>().SetUp(characterDatas[i].ID, characterDatas[i].NAME, characterDatas[i].WorldName, this);
            }
        }
    }

    public void PlayWithPlayer(int id)
    {
        for (int i = 0; i < characterDatas.Count; i++)
        {
            if (characterDatas[i].ID == id)
            {
                Game.ConsoleInGame.LoadingScreen_Show();

                Game.GameManager.Player.UserName = characterDatas[i].NAME;
                Game.GameManager.Player.UserID = characterDatas[i].ID.ToString();

                Game.GameManager.WorldName = characterDatas[i].WorldName;
                Game.GameManager.SetUpSinglePlayer(characterDatas[i].Seed);
                return;
            }
        }
    }

    public void CreateNewChar()
    {
        if (characterDatas.Count < 8)
        {
            Game.ConsoleInGame.LoadingScreen_Show();

            string seed_hash = Random.Range(-99999, 99999).GetHashCode().ToString();

            if (Input_WorldName.text == string.Empty)
            {
                Input_WorldName.text = "World_" + seed_hash;
            }

            if (Input_SeedName.text == string.Empty)
            {
                Input_SeedName.text = seed_hash;
            }
            else
            {
                Input_SeedName.text = Input_SeedName.text.GetHashCode().ToString();
            }

            characterDatas.Add(new CharacterLista(Random.Range(-9999, 9999).GetHashCode(), Input_UserName.text, Input_WorldName.text, Input_SeedName.text));
            Input_UserName.text = "";
            CharCreatorWindow.SetActive(false);
            RenderList();
            SaveWorld.SaveChars(characterDatas.ToArray());

            CreateNewWorldPlay();
        }
    }

    public void CreateNewWorldPlay()
    {
        List.Add(new WorldList(Input_WorldName.text, "", "", Input_SeedName.text));
        SaveInfo(List.ToArray());

        Game.GameManager.Player.UserName = characterDatas[characterDatas.Count - 1].NAME;
        Game.GameManager.Player.UserID = characterDatas[characterDatas.Count -1].ID.ToString();

        Game.GameManager.WorldName = Input_WorldName.text;
        Game.GameManager.SetUpSinglePlayer(Input_SeedName.text);
    }

    public void ClearCanvas()
    {
        foreach (Transform child in Root.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void UserNameInputChange()
    {
        Button_CharCreate.interactable = !string.IsNullOrEmpty(Input_UserName.text);
    }

    public void DeleteOne(int Id)
    {
        int removeid = -1;
        for (int i = 0; i < characterDatas.Count; i++)
        {
            if (characterDatas[i].ID == Id)
            {
                removeid = i;
            }
        }

        DeleteWorldFolder(characterDatas[removeid].WorldName);

        if (removeid != -1)
        {
            characterDatas.RemoveAt(removeid);
        }

        RenderList();
        SaveWorld.SaveChars(characterDatas.ToArray());
    }

    public void ChangeEyesColor(int index)
    {
        Game.GameManager.charcustom.CurrentEyesColor = Game.GameManager.charcustom.Eyes[index];
        RefreshAtlas(Resources.LoadAll<Sprite>("Character/HumanChar"));
    }

    public void ChangeSkinColor(int index)
    {
        Game.GameManager.charcustom.CurrentSkinColor = Game.GameManager.charcustom.Skin[index];
        RefreshAtlas(Resources.LoadAll<Sprite>("Character/HumanChar"));
    }

    public void RefreshAtlas(Sprite[] AnimBase)
    {
        Game.GameManager.charcustom.CharSprites.Clear();

        foreach (var sprite in AnimBase)
        {
            Game.GameManager.charcustom.CharSprites.Add(sprite);
        }

        /*foreach (var sprite in Game.GameManager.charcustom.CharSprites)
        {
            for (int x = 0; x < sprite.texture.width; x++)
            {
                for (int y = 0; y < sprite.texture.height; y++)
                {
                    Color color = sprite.texture.GetPixel(x, y);

                    if (color == Game.Color("00FF04"))//Olhos
                    {
                        sprite.texture.SetPixel(x, y, Game.GameManager.charcustom.CurrentEyesColor.NormalColor);
                    }
                    else if (color == Game.Color("009605"))//Skin
                    {
                        sprite.texture.SetPixel(x, y, Game.GameManager.charcustom.CurrentSkinColor.NormalColor);
                    }
                    else if (color == Game.Color("00AF05") || color == Game.Color("AD009E"))//Torso
                    {
                        sprite.texture.SetPixel(x, y, Game.GameManager.charcustom.CurrentSkinColor.DarkColor);
                    }
                    else if (color == Game.Color("007001"))//Nariz
                    {
                        sprite.texture.SetPixel(x, y, Game.GameManager.charcustom.CurrentSkinColor.DarkColor);
                    }
                    else if (color == Game.Color("E000CD"))//skinWite
                    {
                        sprite.texture.SetPixel(x, y, Game.GameManager.charcustom.CurrentSkinColor.LightColor);
                    }
                    else
                    {
                        sprite.texture.SetPixel(x, y, color);
                    }
                }
            }
            sprite.texture.Apply();
        }*/
    }

    public static Texture2D textureFromSprite(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }

    #region InfoGeral
    public static void SaveInfo(WorldList[] info)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Path.GetFullPath("Saves./SavesData.data"));

        bf.Serialize(file, info);
        file.Close();
    }
    public static WorldList[] LoadInfo()
    {
        if (File.Exists(Path.GetFullPath("Saves./SavesData.data")))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Path.GetFullPath("Saves./SavesData.data"), FileMode.Open);

            WorldList[] dataa = (WorldList[])bf.Deserialize(file);
            file.Close();

            return dataa;
        }

        return null;
    }

    public static void DeleteWorldFolder(string worldName)
    {
        Directory.Delete(Path.GetFullPath("Saves./" + worldName + "./"), true);
    }
    #endregion

}

[System.Serializable]
public class CharacterLista
{
    public int ID;
    public string NAME;
    public string WorldName;
    public string Seed;

    public CharacterLista(int id, string name_char, string world_name, string seed)
    {
        ID = id;
        NAME = name_char;
        WorldName = world_name;
        Seed = seed;
    }
}

[System.Serializable]
public struct CharColorStruc
{
    public Color NormalColor;
    public Color LightColor;
    public Color DarkColor;


    public string GetData()
    {
        return "Soon";
    }
}