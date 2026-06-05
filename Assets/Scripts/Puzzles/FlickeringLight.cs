using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class FlickeringLight : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private float minIntensity = 0.1f;
    [SerializeField] private float maxIntensity = 3f;

    [Header("Timing")]
    [SerializeField] private float minTime = 0.03f;
    [SerializeField] private float maxTime = 0.25f;

    [Header("Blink Behaviour")]
    [SerializeField] private bool randomFlicker = true;
    [SerializeField] private bool sometimesTurnsOff = true;

    private Light lightSource;
    private float originalIntensity;

    private void Awake()
    {
        lightSource = GetComponent<Light>();
        originalIntensity = lightSource.intensity;
    }

    private void OnEnable()
    {
        StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            if (randomFlicker)
            {
                if (sometimesTurnsOff && Random.value < 0.25f)
                {
                    lightSource.intensity = 0f;
                }
                else
                {
                    lightSource.intensity = Random.Range(minIntensity, maxIntensity);
                }

                yield return new WaitForSeconds(Random.Range(minTime, maxTime));
            }
            else
            {
                lightSource.intensity = maxIntensity;
                yield return new WaitForSeconds(maxTime);

                lightSource.intensity = minIntensity;
                yield return new WaitForSeconds(minTime);
            }
        }
    }

    public void ResetLight()
    {
        lightSource.intensity = originalIntensity;
    }
}
