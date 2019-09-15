using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMesh : MonoBehaviour {

    Renderer render;

    public int width = 256;
    public int height = 256;

    public int XX = 0;
    public int YY = 0;

    public Texture2D Text01;

    void Start()
    {
        render = GetComponent<Renderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Texture2D text = GenerateTexture();
            GetComponent<Renderer>().material.mainTexture = text;
            GetComponent<Renderer>().material.SetTexture("_Splat0", text);
            GetComponent<Renderer>().material.SetTexture("_Control", text);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SetTile(XX, YY);
        }
    }

    int c = 0;
    int cy = 0;

    public void SetTile(int xx, int yy)
    {
        Texture2D texture = (Texture2D)GetComponent<Renderer>().material.mainTexture;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x + xx, y + yy, new Color(Text01.GetPixel(x + xx, y + yy).r, Text01.GetPixel(x + xx, y + yy).g, Text01.GetPixel(x + xx, y + yy).b, 0.5f));
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        GetComponent<Renderer>().material.mainTexture = texture;
        GetComponent<Renderer>().material.SetTexture("_Splat0", texture);
        GetComponent<Renderer>().material.SetTexture("_Control", texture);
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(250, 250);
        c = 0;
        cy = 0;
        Color color = Color.clear;
        for (int e =0; e < 10; e++)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Random.Range(0, 10) >= 8)
                {
                    color = Color.green;
                }
                else
                {
                    color = Color.red;
                }

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        texture.SetPixel(x + cy, y + c, color);
                    }
                }
                c += 25;
            }
            c = 0;
            cy += 25;
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        return texture;
    }
}
