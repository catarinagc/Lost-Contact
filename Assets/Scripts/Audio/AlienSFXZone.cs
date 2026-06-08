using UnityEngine;

public class AlienSFXZone : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip alienSFX;

    [SerializeField] private bool playOnlyOnce = true;
    [SerializeField] private string playerTag = "Player";

    private bool hasPlayed = false;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (playOnlyOnce && hasPlayed)
            return;

        PlayAlienSFX();
    }

    private void PlayAlienSFX()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource assigned to AlienSFXZone.");
            return;
        }

        if (alienSFX != null)
            audioSource.PlayOneShot(alienSFX);
        else
            audioSource.Play();

        hasPlayed = true;
    }
}
