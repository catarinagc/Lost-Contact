using UnityEngine;
using System.Collections;

public class DistantFigureJumpscare : MonoBehaviour
{
    [Header("Figure")]
    [SerializeField] private GameObject figure;
    [SerializeField] private float visibleTime = 0.5f;
    [SerializeField] private bool onlyOnce = true;
    [SerializeField] private bool facePlayer = true;

    [Header("Optional Effects")]
    [SerializeField] private AudioSource scareSound;
    [SerializeField] private Light lightToFlicker;
    [SerializeField] private float flickerDuration = 0.4f;

    private bool hasTriggered = false;

    private void Start()
    {
        if (figure != null)
            figure.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (onlyOnce && hasTriggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        hasTriggered = true;
        StartCoroutine(DoJumpscare(other.transform));
    }

    private IEnumerator DoJumpscare(Transform player)
    {
        if (figure != null)
        {
            figure.SetActive(true);

            if (facePlayer)
            {
                Vector3 direction = player.position - figure.transform.position;
                direction.y = 0f;

                if (direction != Vector3.zero)
                    figure.transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        if (scareSound != null)
            scareSound.Play();

        if (lightToFlicker != null)
            StartCoroutine(FlickerLight());

        yield return new WaitForSeconds(visibleTime);

        if (figure != null)
            figure.SetActive(false);
    }

    private IEnumerator FlickerLight()
    {
        float originalIntensity = lightToFlicker.intensity;
        float elapsed = 0f;

        while (elapsed < flickerDuration)
        {
            lightToFlicker.intensity = 0f;
            yield return new WaitForSeconds(0.06f);

            lightToFlicker.intensity = originalIntensity;
            yield return new WaitForSeconds(0.08f);

            elapsed += 0.14f;
        }

        lightToFlicker.intensity = originalIntensity;
    }
}
