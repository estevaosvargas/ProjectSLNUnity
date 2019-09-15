using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class TriggerAction : MonoBehaviour {

    public List<string> Tags = new List<string>();

    [HideInInspector]
    public Collider EnterCollider;
    [HideInInspector]
    public Collider ExitCollider;
    [HideInInspector]
    public Collider StayCollider;

    [Header("Script Events")]
    public UnityEvent EnterAction;
    public UnityEvent ExitAction;
    [Tooltip("Maybe this cause some lag, because is update fuction, on collision system")]
    public UnityEvent StayAction;

    private BoxCollider boxCollider;

    private void Reset()
    {
        var col = GetComponent<BoxCollider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;

        boxCollider = col;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Tags.Contains(other.tag))
        {
            EnterCollider = other;
            EnterAction.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Tags.Contains(other.tag))
        {
            ExitCollider = other;
            ExitAction.Invoke();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Tags.Contains(other.tag))
        {
            StayCollider = other;
            StayAction.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);

        Gizmos.DrawCube(transform.position + boxCollider.center, new Vector3(boxCollider.size.x * transform.localScale.x, boxCollider.size.y * transform.localScale.y, boxCollider.size.z * transform.localScale.z));
    }
}
