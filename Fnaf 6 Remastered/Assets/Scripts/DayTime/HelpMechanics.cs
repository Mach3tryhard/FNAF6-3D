using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class HelpMechanics : MonoBehaviour
{
    public ScriptableRendererFeature occludedFeature;
    private bool isOccludedActive = false;
    public Material HaxMaterial;

    // Variables for controlling the pulse effect
    public Color color1 = Color.blue;
    public Color color2 = Color.white;
    private float pulseDuration = 0.5f;  // Time for one pulse cycle
    private int pulseCount = 20;          // Number of times the pulse effect occurs

    void Start()
    {
        occludedFeature.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if(isOccludedActive==false)
            {
                occludedFeature.SetActive(true);
                isOccludedActive=true;
                StartCoroutine(PulseEffect());
            }
        }
    }

    private IEnumerator PulseEffect()
    {
        HaxMaterial.EnableKeyword("_EMISSION");

        for (int i = 0; i < pulseCount; i++)
        {
            float elapsedTime = 0f;
            while (elapsedTime < pulseDuration)
            {
                float t = elapsedTime / pulseDuration;
                Color emissionColor = Color.Lerp(color1, color2, Mathf.PingPong(t * 2f, 1f)); // PingPong makes it oscillate
                HaxMaterial.SetColor("_EmissionColor", emissionColor);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        //yield return new WaitForSeconds(3f);

        isOccludedActive=false;
        occludedFeature.SetActive(false);
    }
}
