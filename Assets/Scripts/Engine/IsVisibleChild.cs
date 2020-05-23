using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsVisibleChild : MonoBehaviour
{
    public Entity Entity;

    private void OnBecameVisible()
    {
        Entity.BecameVisible();
    }

    private void OnBecameInvisible()
    {
        Entity.BecameInvisible();
    }
}
