using UnityEngine;

public class PuzzleSFX : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("SFX")]
    [SerializeField] private AudioClip openSFX;
    [SerializeField] private AudioClip closeSFX;
    [SerializeField] private AudioClip clickSFX;
    [SerializeField] private AudioClip correctSFX;
    [SerializeField] private AudioClip wrongSFX;
    [SerializeField] private AudioClip solvedSFX;

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.8f;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Play(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip, volume);
    }

    public void PlayOpen()
    {
        Play(openSFX);
    }

    public void PlayClose()
    {
        Play(closeSFX);
    }

    public void PlayClick()
    {
        Play(clickSFX);
    }

    public void PlayCorrect()
    {
        Play(correctSFX);
    }

    public void PlayWrong()
    {
        Play(wrongSFX);
    }

    public void PlaySolved()
    {
        Play(solvedSFX);
    }
}