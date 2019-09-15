using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class PressKeyScreen : MonoBehaviour
{
    public Text TextKey;
    public GameObject MainMenu;
    public Animator Anim;

    public float FadeDuration = 1;
    public float SecondsToDisable = 1;

    public KeyCode PC = KeyCode.Space;

    public bool Pressed = false;

    private void Start()
    {
        TextKey.text = "Press " + PC + " To Start";
    }

    private void Update()
    {
        if (Pressed == false)
        {
            TextKey.color = new Color(TextKey.color.r, TextKey.color.g, TextKey.color.b, Mathf.PingPong(Time.time * FadeDuration, 1));

            if (Input.GetKeyDown(PC))
            {
                Press(false);
            }
        }
    }

    public void Press(bool skip)
    {
        if (skip != true)
        {
            StartCoroutine(playanim());
            StopCoroutine(playanim());
        }
        else
        {
            gameObject.SetActive(false);
            MainMenu.SetActive(true);
            Pressed = true;
        }
    }

    IEnumerator playanim()
    {
        Anim.SetTrigger("Pressed");
        Pressed = true;

        yield return new WaitForSeconds(SecondsToDisable);
        Anim.enabled = false;
        gameObject.SetActive(false);
        MainMenu.SetActive(true);
    }
}
