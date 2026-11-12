using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public float flickerIntensity = 0.2f;
    public float flickerPerSecond = 3.0f;
    public float speedRandomness = 1.0f;

    private float time;
    private float startingIntensity;
    private Light targetLight;

    void Start()
    {
        targetLight = GetComponent<Light>();
        startingIntensity = targetLight.intensity;
    }

    void Update()
    {
        time += Time.deltaTime * (1 - Random.Range(-speedRandomness,speedRandomness)) * Mathf.PI;
        targetLight.intensity = startingIntensity * Mathf.Sin(time * flickerPerSecond) * flickerIntensity;
    }
}
