using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise.Unity.Generator;

public class NoisePreview : MonoBehaviour
{
    public enum PerlinTypes { Billow, BrownianMotion , Checker, Const, Cylinders, HeterogeneousMultiFractal, HybridMulti, Perlin, RiggedMultifractal, Spheres, Voronoi, UnityPerlin, turbulence, Terrace, PerlinRandomObject }

    public PerlinTypes Types;
    public FilterMode FildterMode = FilterMode.Bilinear;
    public LibNoise.Unity.QualityMode Quality;

    public int width = 256;
    public int height = 256;

    public float Scale = 20f;
    public float Offset = 100f;
    public float noisefactor = 0.1f;

    public double Power;

    public bool UseIamage = false;

    public bool distancevo = false;

    public Texture2D HeightTeste;

    public float mean, standard_deviation, min, max;

    Renderer render;
    void Start()
    {
        render = GetComponent<Renderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            render.material.mainTexture = GenerateTexture();
        }
        float mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        float mouseY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
        if (UseIamage == true)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                render.material.mainTexture = HeightTeste;

                Debug.Log(HeightTeste.GetPixel((int)mouseX, (int)mouseY));
            }
        }

        render.material.mainTexture.filterMode = FildterMode;
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x,y, color);
            }
        }
        texture.Apply();
        return texture;
    }

    public float persistence  = 1.0f;
    public float frequency = 1.0f;
    public float amplitude  = 1.0f;
    public int octaves = 1;
    public int randomSeed = 1;

    public float lacunarit = 1;
    public float gain = 1;
    public float displacemt = 1;

    Color CalculateColor(int x, int y)
    {
        /*float xCorde = (float)octaves * x / width * Scale + randomSeed;
        float yCorde = (float)octaves * y / height * Scale + randomSeed;

        // modify with frequency
        xCorde *= frequency;
        yCorde *= frequency;*/
        float sample = 0;

   
        switch (Types)
        {
            case PerlinTypes.Billow:
                sample = (float)new Billow(frequency, lacunarit, persistence, octaves, randomSeed, Quality).GetValue(x, y, 0);
                break;
            case PerlinTypes.BrownianMotion:
                sample = (float)new BrownianMotion(frequency, lacunarit, octaves, randomSeed, Quality).GetValue(x, y, 0);
                break;
            case PerlinTypes.Checker:
                sample = (float)new Checker().GetValue(x * frequency / octaves, y * frequency / octaves, 0);
                break;
            case PerlinTypes.Const:
                sample = (float)new Const(frequency).GetValue(x, y, 0);
                break;
            case PerlinTypes.Cylinders:
                sample = (float)new Cylinders(frequency).GetValue(x, y, 0);
                break;
            case PerlinTypes.HeterogeneousMultiFractal:
                sample = (float)new HeterogeneousMultiFractal(frequency, lacunarit, octaves, persistence, randomSeed, Offset, Quality).GetValue(x, y, 0);
                break;
            case PerlinTypes.HybridMulti:
                sample = (float)new HybridMulti(frequency, lacunarit, octaves, persistence, randomSeed, Offset, gain, Quality).GetValue(x, y, 0);
                break;
            case PerlinTypes.Perlin:
                sample = (float)new Perlin(frequency, lacunarit, persistence, octaves, randomSeed, Quality).GetValue(x, y, 0);
                break;
            case PerlinTypes.RiggedMultifractal:
                sample = (float)new RiggedMultifractal(frequency, lacunarit, octaves, randomSeed, Quality).GetValue(x, y, 0);
                break;
            case PerlinTypes.Spheres:
                sample = (float)new Spheres(frequency).GetValue(x, y, 0);
                break;
            case PerlinTypes.Voronoi:
                sample = (float)new Voronoi(frequency, displacemt, randomSeed, distancevo).GetValue(x + Offset, y + Offset, 0);
                break;
            case PerlinTypes.UnityPerlin:
                sample = Mathf.PerlinNoise(x + Offset, y + Offset);
                break;
            case PerlinTypes.turbulence:
                sample = (float)new LibNoise.Unity.Operator.Turbulence().GetValue(x, y, 0);
                break;
            case PerlinTypes.Terrace:
                sample = (float)new LibNoise.Unity.Operator.Terrace().GetValue(x, y, 0);
                break;
            case PerlinTypes.PerlinRandomObject:
                sample = NextGaussian(mean, standard_deviation, min, max);
                break;
            default:
                sample = (float)new Perlin(frequency, lacunarit, persistence, octaves, randomSeed, Quality).GetValue(x, y, 0);
                break;
        }
       

        //sample *= amplitude;

        return new Color(sample, sample, sample);
    }

    public static float NextGaussian()
    {
        float v1, v2, s;
        do
        {
            v1 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            v2 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0f);

        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

        return v1 * s;
    }

    public static float NextGaussian(float mean, float standard_deviation)
    {
        return mean + NextGaussian() * standard_deviation;
    }

    public static float NextGaussian(float mean, float standard_deviation, float min, float max)
    {
        float x;
        do
        {
            x = NextGaussian(mean, standard_deviation);
        }
        while (x < min || x > max);
        return x;
    }
}