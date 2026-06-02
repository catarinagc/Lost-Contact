using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StaticOverlayController : MonoBehaviour
{
    [SerializeField] private RawImage staticImage;

    [Header("Static Settings")]
    [SerializeField] private int textureSize = 128;
    [SerializeField] private float updateInterval = 0.08f;

    [Range(0f, 0.2f)]
    [SerializeField] private float normalOpacity = 0.04f;

    private Texture2D staticTexture;
    private Color32[] pixels;
    private float timer;
    private Coroutine glitchRoutine;

    private void Start()
    {
        if (staticImage == null)
            staticImage = GetComponent<RawImage>();

        staticTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        staticTexture.filterMode = FilterMode.Point;
        staticTexture.wrapMode = TextureWrapMode.Repeat;

        pixels = new Color32[textureSize * textureSize];

        staticImage.texture = staticTexture;

        SetOpacity(normalOpacity);
        GenerateStatic();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            timer = 0f;
            GenerateStatic();
        }
    }

    private void GenerateStatic()
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            byte value = (byte)Random.Range(30, 180);
            pixels[i] = new Color32(value, value, value, 255);
        }

        staticTexture.SetPixels32(pixels);
        staticTexture.Apply();
    }

    public void TriggerGlitch(float duration = 0.5f, float glitchOpacity = 0.12f)
    {
        if (glitchRoutine != null)
            StopCoroutine(glitchRoutine);

        glitchRoutine = StartCoroutine(GlitchRoutine(duration, glitchOpacity));
    }

    private IEnumerator GlitchRoutine(float duration, float glitchOpacity)
    {
        SetOpacity(glitchOpacity);

        float originalInterval = updateInterval;
        updateInterval = 0.03f;

        yield return new WaitForSeconds(duration);

        updateInterval = originalInterval;
        SetOpacity(normalOpacity);

        glitchRoutine = null;
    }

    private void SetOpacity(float opacity)
    {
        if (staticImage == null)
            return;

        Color c = staticImage.color;
        c.a = opacity;
        staticImage.color = c;
    }
}