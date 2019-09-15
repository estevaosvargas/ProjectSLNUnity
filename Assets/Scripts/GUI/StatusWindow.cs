using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatusWindow : MonoBehaviour
{
    public static StatusWindow Instance;
    public SkillStruc[] SkillsList;
    public WepoSkillStruc[] WeapondSkill;
    public GameObject SkillText;

    public RectTransform Root1;
    public RectTransform Root2;

    public int Size = 5;
    public float Spacing = 3;

    public Text Title1;
    public Text Title2;

    public PagesNum Pages;

    private void OnEnable()
    {
        Instance = this;
        if (Game.GameManager.MyPlayer.MyPlayerMove)
        {
            switch (Pages)
            {
                case PagesNum.index:

                    break;
                case PagesNum.Page01:
                    ClearCanvas();
                    if (Game.GameManager.MyPlayer.MyPlayerMove.Status.SkillsList.Count > 0)
                    {
                        SkillsList = Game.GameManager.MyPlayer.MyPlayerMove.Status.SkillsList.ToArray();
                        DrawnList01(SkillsList);
                    }
                    if (Game.GameManager.MyPlayer.MyPlayerMove.Status.WPSkillsList.Count > 0)
                    {
                        WeapondSkill = Game.GameManager.MyPlayer.MyPlayerMove.Status.WPSkillsList.ToArray();
                        DrawnList01(WeapondSkill);
                    }

                    break;
                case PagesNum.Page02:
                    break;
                default:
                    if (Game.GameManager.MyPlayer.MyPlayerMove.Status.SkillsList.Count > 0)
                    {
                        SkillsList = Game.GameManager.MyPlayer.MyPlayerMove.Status.SkillsList.ToArray();
                        DrawnList01(SkillsList);
                    }
                    break;
            }
        }
    }

    public void Refresh()
    {
        if (Game.GameManager.MyPlayer.MyPlayerMove)
        {
            switch (Pages)
            {
                case PagesNum.index:

                    break;
                case PagesNum.Page01:
                    ClearCanvas();
                    if (Game.GameManager.MyPlayer.MyPlayerMove.Status.SkillsList.Count > 0)
                    {
                        SkillsList = Game.GameManager.MyPlayer.MyPlayerMove.Status.SkillsList.ToArray();
                        DrawnList01(SkillsList);
                    }
                    if (Game.GameManager.MyPlayer.MyPlayerMove.Status.WPSkillsList.Count > 0)
                    {
                        WeapondSkill = Game.GameManager.MyPlayer.MyPlayerMove.Status.WPSkillsList.ToArray();
                        DrawnList01(WeapondSkill);
                    }

                    break;
                case PagesNum.Page02:
                    break;
                default:
                    if (Game.GameManager.MyPlayer.MyPlayerMove.Status.SkillsList.Count > 0)
                    {
                        SkillsList = Game.GameManager.MyPlayer.MyPlayerMove.Status.SkillsList.ToArray();
                        DrawnList01(SkillsList);
                    }
                    break;
            }
        }
    }

    public void DrawnList01(SkillStruc[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(SkillText, Vector3.zero, Quaternion.identity);

            obj.GetComponent<Text>().text = list[i].Type.ToString() + " : (LV: " + list[i].SkillLevel + ") " + list[i].SkillXp + "/" + list[i].MaxSkillXp;

            obj.transform.SetParent(Root1.gameObject.transform);
            RectTransform rect = obj.GetComponent<RectTransform>();
        }
    }

    public void DrawnList01(WepoSkillStruc[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(SkillText, Vector3.zero, Quaternion.identity);

            obj.GetComponent<Text>().text = ItemManager.Instance.GetItem(list[i].Itemid).Name + " : (LV: " + list[i].SkillLevel + ") " + list[i].SkillXp + "/" + list[i].MaxSkillXp;

            obj.transform.SetParent(Root2.gameObject.transform);
            RectTransform rect = obj.GetComponent<RectTransform>();
        }
    }

    public void ClearCanvas()
    {
        foreach (Transform child in Root1.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in Root2.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
//remover depois
public enum PagesStatus
{
    index, playerstatus, weapondsstatus, toolsstatus
}

public enum PagesNum
{
    index, Page01, Page02, Page03
}