using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveGameItem : MonoBehaviour, IPointerClickHandler
{
    public Text Name;
    public Text Descripto;
    public SinglePlayerMenu Menu;
    public string Seed;
    public GameObject Selected;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Menu.Corrent != null)
        {
            if (Menu.Corrent != this)
            {
                Menu.Corrent.Selected.SetActive(false);
                Selected.SetActive(true);
                Menu.Corrent = this;
            }
            else
            {
                Menu.PlayButton.SetActive(true);
                Menu.Corrent = this;
                Selected.SetActive(true);
            }
        }
        else
        {
            Menu.PlayButton.SetActive(true);
            Menu.Corrent = this;
            Selected.SetActive(true);
        }
    }

    public void Setup(string name, string descrip,string seed, SinglePlayerMenu menu)
    {
        Name.text = name;
        Descripto.text = descrip;
        Menu = menu;
        Seed = seed;
    } 
}