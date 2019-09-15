using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConsoleItem : MonoBehaviour, IPointerClickHandler
{
    public Text Text;

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void Setup(string text)
    {
        Text.text = text;
    } 
}