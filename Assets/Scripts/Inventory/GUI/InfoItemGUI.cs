using UnityEngine.UI;
using UnityEngine;

public class InfoItemGUI : MonoBehaviour
{

    public Text Name;
    public Text Description;
    public Text rarityText;

    public Image Icon;
     

    public void SetInfo(string name, string desciption, Sprite icon, string rarity,Color colortext)
    {
        Name.color = colortext;
        rarityText.color = colortext;
        Name.text = name;
        Description.text = desciption;
        Icon.sprite = icon;
        rarityText.text = rarity;
    }
}