using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    public float scale = 0.1f;
    public float speed = 1.0f;
    //The width between the waves
    public float waveDistance = 1f;
    //Noise parameters
    public float noiseStrength = 1f;
    public float noiseWalk = 1f;

    private Mesh mesh;


    void Update()
    {
        AnimateMesh();
    }

    void AnimateMesh()
    {
        if (!mesh)
            mesh = GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            //float pX = (vertices[i].x * perlinScale) + (Time.timeSinceLevelLoad * waveSpeed);
            //float pZ = (vertices[i].z * perlinScale) + (Time.timeSinceLevelLoad * waveSpeed);

            //vertices[i].y = (Mathf.PerlinNoise(pX, pZ) - 0.5f) * waveHeight;
            vertices[i].y = WaveTypes.SinXWave(new Vector3(vertices[i].x, 0, vertices[i].z), speed, scale, waveDistance, noiseStrength, noiseWalk, Time.time);
        }

        mesh.vertices = vertices;
    }
}

//Different wavetypes
public class WaveTypes
{

    //Sinus waves
    public static float SinXWave( Vector3 position,float speed, float scale, float waveDistance, float noiseStrength,float noiseWalk,float timeSinceStart)
    {
        float x = position.x;
        float y = 0f;
        float z = position.z;

        //Using only x or z will produce straight waves
        //Using only y will produce an up/down movement
        //x + y + z rolling waves
        //x * z produces a moving sea without rolling waves

        float waveType = z;

        y += Mathf.Sin((timeSinceStart * speed + waveType) / waveDistance) * scale;

        //Add noise to make it more realistic
        y += Mathf.PerlinNoise(x + noiseWalk, y + Mathf.Sin(timeSinceStart * 0.1f)) * noiseStrength;

        return y;
    }
}