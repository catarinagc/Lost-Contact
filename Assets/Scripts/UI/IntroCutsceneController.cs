using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class IntroCutsceneController : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text mainText;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private TMP_Text titleText;

    [Header("Static Overlay")]
    [SerializeField] private RawImage staticOverlay;
    [SerializeField] private int staticTextureSize = 128;
    [SerializeField] private float staticUpdateInterval = 0.08f;

    [Range(0f, 0.2f)]
    [SerializeField] private float normalStaticOpacity = 0.04f;

    [Range(0f, 0.2f)]
    [SerializeField] private float glitchStaticOpacity = 0.12f;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource beepSource;
    [SerializeField] private AudioSource glitchSource;

    [Header("Scene")]
    [SerializeField] private string nextSceneName = "GameScene";

    [Header("Timing")]
    [SerializeField] private float letterDelay = 0.035f;
    [SerializeField] private float linePause = 1.1f;
    [SerializeField] private float finalTitleDuration = 3f;

    [Header("Intro Lines")]
    [TextArea(2, 5)]
    [SerializeField] private string[] introLines;

    private bool isSkipping = false;

    private Texture2D staticTexture;
    private Color32[] staticPixels;
    private float staticTimer;
    private float currentStaticUpdateInterval;
    private Coroutine glitchRoutine;

    private void Start()
    {
        if (mainText != null)
            mainText.text = "";

        if (warningText != null)
            warningText.gameObject.SetActive(false);

        if (titleText != null)
            titleText.gameObject.SetActive(false);

        SetupStaticOverlay();

        if (musicSource != null)
            musicSource.Play();

        StartCoroutine(PlayIntro());
    }

    private void Update()
    {
        UpdateStaticOverlay();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isSkipping)
            {
                isSkipping = true;
                StopAllCoroutines();
                StartCoroutine(EndIntro());
            }
        }
    }

    private IEnumerator PlayIntro()
    {
        for (int i = 0; i < introLines.Length; i++)
        {
            if (mainText != null)
                mainText.text = "";

            yield return StartCoroutine(TypeLine(introLines[i]));

            if (ShouldShowWarning(introLines[i]))
            {
                yield return StartCoroutine(ShowWarning("CONTACT LOST"));
            }

            yield return new WaitForSeconds(linePause);
        }

        yield return StartCoroutine(EndIntro());
    }

    private IEnumerator TypeLine(string line)
    {
        foreach (char c in line)
        {
            if (mainText != null)
                mainText.text += c;

            if (beepSource != null && c != ' ')
                beepSource.Play();

            // Rare safe static glitch while typing
            if (Random.value < 0.003f)
                TriggerStaticGlitch(0.15f);

            yield return new WaitForSeconds(letterDelay);
        }
    }

    private bool ShouldShowWarning(string line)
    {
        return line.Contains("Contact with the team was lost") ||
               line.Contains("voices disappeared") ||
               line.Contains("self-destruction") ||
               line.Contains("clock is ticking");
    }

    private IEnumerator ShowWarning(string message)
    {
        if (warningText == null)
            yield break;

        warningText.text = message;
        warningText.gameObject.SetActive(true);

        if (glitchSource != null)
            glitchSource.Play();

        TriggerStaticGlitch(0.7f);

        yield return new WaitForSeconds(0.7f);

        warningText.gameObject.SetActive(false);
    }

    private IEnumerator EndIntro()
    {
        if (mainText != null)
            mainText.text = "";

        if (warningText != null)
            warningText.gameObject.SetActive(false);

        if (titleText != null)
            titleText.gameObject.SetActive(true);

        if (glitchSource != null)
            glitchSource.Play();

        TriggerStaticGlitch(1f);

        yield return new WaitForSeconds(finalTitleDuration);

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    private void SetupStaticOverlay()
    {
        if (staticOverlay == null)
            return;

        currentStaticUpdateInterval = staticUpdateInterval;

        staticTexture = new Texture2D(staticTextureSize, staticTextureSize, TextureFormat.RGBA32, false);
        staticTexture.filterMode = FilterMode.Point;
        staticTexture.wrapMode = TextureWrapMode.Repeat;

        staticPixels = new Color32[staticTextureSize * staticTextureSize];

        staticOverlay.texture = staticTexture;
        SetStaticOpacity(normalStaticOpacity);
        GenerateStatic();
    }

    private void UpdateStaticOverlay()
    {
        if (staticOverlay == null || staticTexture == null)
            return;

        staticTimer += Time.deltaTime;

        if (staticTimer >= currentStaticUpdateInterval)
        {
            staticTimer = 0f;
            GenerateStatic();
        }
    }

    private void GenerateStatic()
    {
        if (staticPixels == null || staticTexture == null)
            return;

        for (int i = 0; i < staticPixels.Length; i++)
        {
            byte value = (byte)Random.Range(30, 180);
            staticPixels[i] = new Color32(value, value, value, 255);
        }

        staticTexture.SetPixels32(staticPixels);
        staticTexture.Apply();
    }

    private void TriggerStaticGlitch(float duration)
    {
        if (staticOverlay == null)
            return;

        if (glitchRoutine != null)
            StopCoroutine(glitchRoutine);

        glitchRoutine = StartCoroutine(StaticGlitchRoutine(duration));
    }

    private IEnumerator StaticGlitchRoutine(float duration)
    {
        SetStaticOpacity(glitchStaticOpacity);
        currentStaticUpdateInterval = 0.03f;

        yield return new WaitForSeconds(duration);

        currentStaticUpdateInterval = staticUpdateInterval;
        SetStaticOpacity(normalStaticOpacity);

        glitchRoutine = null;
    }

    private void SetStaticOpacity(float opacity)
    {
        if (staticOverlay == null)
            return;

        Color c = staticOverlay.color;
        c.a = opacity;
        staticOverlay.color = c;
    }
}