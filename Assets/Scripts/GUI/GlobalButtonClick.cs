using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalButtonClick : MonoBehaviour
{
    public int pageid;

    public void OpenPageLink()
    {
        StatusWindow.Instance.OpenPage(pageid);
    }
}
