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
    public GameObject LinkText;

    public RectTransform Root1;
    public RectTransform Root2;

    public int Size = 5;
    public float Spacing = 3;

    public Text Title1;
    public Text Title2;

    public GameObject NextButtun;
    public GameObject BackButtun;

    public int Pages;
    public int TotalPages = 2;

    private void OnEnable()
    {
        Instance = this;
        Refresh();
    }

    public void Refresh()
    {
        if (Game.GameManager.CurrentPlayer.MyPlayerMove)
        {
            ClearCanvas();
            switch (Pages)
            {
                case 0:
                    Title1.text = "Index 01";
                    Title2.text = "Index 02";

                    for (int i = 0; i <= 2; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                DrawnList01_Link("1:2 - Index 01 & Index 02", i);
                                break;
                            case 1:
                                DrawnList01_Link("3:4 - Professions Skills & Weapons Skills", i);
                                break;
                            case 2:
                                DrawnList01_Link("5:6 - Uhm.... Nothing. & Uhm.... Nothing. too", i);
                                break;
                        }
                    }
                    break;
                case 1:
                    Title1.text = "Professions Skills";
                    Title2.text = "Weapons Skills";

                    if (Game.GameManager.CurrentPlayer.MyPlayerMove.Status.SkillsList.Count > 0)
                    {
                        SkillsList = Game.GameManager.CurrentPlayer.MyPlayerMove.Status.SkillsList.ToArray();

                        for (int i = 0; i < SkillsList.Length; i++)
                        {
                            DrawnList01(SkillsList[i].Type.ToString() + " : (LV: " + SkillsList[i].SkillLevel + ") " + SkillsList[i].SkillXp + "/" + SkillsList[i].MaxSkillXp);
                        }
                    }
                    if (Game.GameManager.CurrentPlayer.MyPlayerMove.Status.WPSkillsList.Count > 0)
                    {
                        WeapondSkill = Game.GameManager.CurrentPlayer.MyPlayerMove.Status.WPSkillsList.ToArray();

                        for (int i = 0; i < WeapondSkill.Length; i++)
                        {
                            DrawnList02(ItemManager.Instance.GetItem(WeapondSkill[i].Itemid).Name + " : (LV: " + WeapondSkill[i].SkillLevel + ") " + WeapondSkill[i].SkillXp + "/" + WeapondSkill[i].MaxSkillXp);
                        }
                    }

                    break;
                case 2:
                    Title1.text = "Uhm.... Nothing.";
                    Title2.text = "Uhm.... Nothing. too";
                    break;
            }
        }
    }

    public void DrawnList01_Link(string text, int pageid)
    {
        GameObject Link_Text = GameObject.Instantiate(LinkText, Vector3.zero, Quaternion.identity);
        Link_Text.GetComponent<TMPro.TextMeshProUGUI>().text = text;
        Link_Text.GetComponent<GlobalButtonClick>().pageid = pageid;
        Link_Text.transform.SetParent(Root1.gameObject.transform);
    }

    public void DrawnList02_Link(string text, int pageid)
    {
        GameObject Link_Text = GameObject.Instantiate(LinkText, Vector3.zero, Quaternion.identity);
        Link_Text.GetComponent<TMPro.TextMeshProUGUI>().text = text;
        Link_Text.GetComponent<GlobalButtonClick>().pageid = pageid;
        Link_Text.transform.SetParent(Root2.gameObject.transform);
    }

    public void DrawnList01(string text)
    {
        GameObject Skill_Text = GameObject.Instantiate(SkillText, Vector3.zero, Quaternion.identity);
        Skill_Text.GetComponent<Text>().text = text;
        Skill_Text.transform.SetParent(Root1.gameObject.transform);
    }

    public void DrawnList02(string text)
    {
        GameObject Skill_Text = GameObject.Instantiate(SkillText, Vector3.zero, Quaternion.identity);
        Skill_Text.GetComponent<Text>().text = text;
        Skill_Text.transform.SetParent(Root2.gameObject.transform);
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

    public void OpenPage(int i)
    {
        GameManager.AudioSourceGlobal.PlayOneShot(Game.AudioManager.GetPageFlipAudio());

        if (i == TotalPages)
        {
            NextButtun.SetActive(false);
            BackButtun.SetActive(true);
        }
        else
        {
            if (i != 0)
            {
                NextButtun.SetActive(true);
                BackButtun.SetActive(true);
            }
            else
            {
                NextButtun.SetActive(true);
                BackButtun.SetActive(false);
            }
        }

        Pages = i;
        Refresh();
    }

    public void Next()
    {
        if (Pages < TotalPages)
        {
            GameManager.AudioSourceGlobal.PlayOneShot(Game.AudioManager.GetPageFlipAudio());
            if (Pages == TotalPages - 1)
            {
                NextButtun.SetActive(false);
                BackButtun.SetActive(true);
            }
            else
            {
                NextButtun.SetActive(true);
                BackButtun.SetActive(true);
            }

            Pages += 1;
            Refresh();
        }
    }

    public void Back()
    {
        if (Pages > 0)
        {
            GameManager.AudioSourceGlobal.PlayOneShot(Game.AudioManager.GetPageFlipAudio());
            if (Pages == 1)
            {
                NextButtun.SetActive(true);
                BackButtun.SetActive(false);
            }
            else
            {
                NextButtun.SetActive(true);
                BackButtun.SetActive(true);
            }

            Pages -= 1;
            Refresh();
        }
    }
}
//remover depois
public enum PagesStatus
{
    index, playerstatus, weapondsstatus, toolsstatus
}

public enum PagesElemente
{
    none, Text, TextLink
}