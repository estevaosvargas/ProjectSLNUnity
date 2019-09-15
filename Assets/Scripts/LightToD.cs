using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeofLight { Point, Direction, Spot, None }

[ExecuteInEditMode]
public class LightToD : MonoBehaviour
{
    public static LightToD Instance;

    public TypeofLight Type = TypeofLight.Point;
    public Color LightColor = Color.white;
    [Range(0, 999)]
    public float Range = 1;
    [Range(0, 1)]
    public float Density = 1;
    public int Relevance = 0;

    private SpriteRenderer Sprite;

    void Start()
    {
        Sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private Vector3 lastposition;
    private Color lastcolor;
    private float lastrange;
    private float lastdensity;

    void Update()
    {
        if (lastposition != transform.position) { RefreshLight(); }
        if (lastcolor != LightColor) { RefreshLight(); }
        if (lastrange != Range) { RefreshLight(); }
        if (lastdensity != Density) { RefreshLight(); }

        lastposition = transform.position;
        lastcolor = LightColor;
        lastrange = Range;
        lastdensity = Density;
    }


    public void RefreshLight()
    {
        if (Type == TypeofLight.Point)
        {
            Sprite.color = new Color(LightColor.r, LightColor.g, LightColor.b, Density);
            transform.localScale = new Vector3(Range, Range, 1);
        }
    }

    void DoneLight()
    {
       
    }

    void OnDestroy()
    {
        DoneLight();
    }

    void OnDisable()
    {
        DoneLight();
    }
}