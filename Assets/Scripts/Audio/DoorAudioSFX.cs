using UnityEngine;

public class DoorAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audioSource;

    [Header("Door Sounds")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    [Header("Settings")]
    [SerializeField] private float volume = 0.8f;

    public void PlayOpenSound()
    {
        if (audioSource == null || openSound == null)
            return;

        audioSource.PlayOneShot(openSound, volume);
    }

    public void PlayCloseSound()
    {
        if (audioSource == null || closeSound == null)
            return;

        audioSource.PlayOneShot(closeSound, volume);
    }
}