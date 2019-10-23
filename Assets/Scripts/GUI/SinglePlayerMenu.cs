using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class SinglePlayerMenu : MonoBehaviour
{
    public List<WorldList> List = new List<WorldList>();
    public RectTransform InveRoot;

    public GameObject SlotPrefab;

    public int Size = 5;
    public float Spacing = 3;

    public InputField NameInput;
    public InputField SeedInput;

    public GameObject PlayButton;

    public SaveGameItem Corrent;

    void OnEnable()
    {
        DrawnList();
    }

    void OnDisable()
    {
        ClearCanvas();
    }

    public void DrawnList()
    {
        ClearCanvas();

        for(int i =0; i < List.Count; i++)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity);

            obj.GetComponent<SaveGameItem>().Setup(List[i].Name, List[i].Description, List[i].Seed, this);

            obj.transform.SetParent(InveRoot.gameObject.transform);
            RectTransform rect = obj.GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(0, -(((rect.sizeDelta.y + Spacing) * i) + Spacing));
            rect.localScale = SlotPrefab.gameObject.transform.localScale;
        }
    }

    void Start()
    {
        if (File.Exists(System.IO.Path.GetFullPath("Saves./SavesData.data")))
        {
            /*foreach (WorldList obj in LoadInfo())
            {
                List.Add(obj);
            }*/

            DrawnList();
        }
    }

    public void ClearCanvas()
    {
        foreach (Transform child in InveRoot.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void PlaySave()
    {
        Game.GameManager.WorldName = Corrent.Name.text;
        Game.GameManager.SetUpSinglePlayer(Corrent.Seed);
    }

    public void CreateSave()
    {
        List.Add(new WorldList(NameInput.text, "", "", SeedInput.text));
        //SaveInfo(List.ToArray());
        Game.GameManager.WorldName = NameInput.text;
        Game.GameManager.SetUpSinglePlayer(SeedInput.text);
    }

    public void DeletSave()
    {
        //List.Remove(new WorldList(name, "", ""));
        //SaveInfo(List.ToArray());
    }
}

[System.Serializable]
public class WorldList
{
    public string Name = "";
    public string Description = "";
    public string id = "";
    public string Seed = "";

    public WorldList(string name, string description, string id, string seed)
    {
        this.Name = name;
        this.Description = description;
        this.id = id;
        this.Seed = seed;
    }
}
