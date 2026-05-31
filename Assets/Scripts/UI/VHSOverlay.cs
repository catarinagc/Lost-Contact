using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VHSOverlay : MonoBehaviour
{
    [Header("Overlay Images")]
    [SerializeField] private RawImage staticImage;
    [SerializeField] private RawImage scanlineImage;

    [Header("Text")]
    [SerializeField] private TMP_Text recText;
    [SerializeField] private TMP_Text timestampText;

    [Header("Static Settings")]
    [SerializeField] private int noiseSize = 128;
    [SerializeField] private float staticRefreshRate = 0.05f;
    [SerializeField] private float staticOpacity = 0.06f;

    [Header("Scanline Settings")]
    [SerializeField] private int scanlineTextureHeight = 64;
    [SerializeField] private float scanlineOpacity = 0.10f;
    [SerializeField] private float scanlineScrollSpeed = 0.15f;

    [Header("REC Blink")]
    [SerializeField] private float recBlinkSpeed = 0.6f;

    private Texture2D noiseTexture;
    private Texture2D scanlineTexture;
    private float staticTimer;
    private float gameTimer;

    void Start()
    {
        CreateNoiseTexture();
        CreateScanlineTexture();

        if (staticImage != null)
        {
            staticImage.texture = noiseTexture;
            staticImage.color = new Color(1f, 1f, 1f, staticOpacity);
            staticImage.raycastTarget = false;
        }

        if (scanlineImage != null)
        {
            scanlineImage.texture = scanlineTexture;
            scanlineImage.color = new Color(1f, 1f, 1f, scanlineOpacity);
            scanlineImage.raycastTarget = false;
        }
    }

    void Update()
    {
        gameTimer += Time.deltaTime;

        UpdateStatic();
        UpdateScanlines();
        UpdateTimestamp();
        UpdateREC();
    }

    private void CreateNoiseTexture()
    {
        noiseTexture = new Texture2D(noiseSize, noiseSize);
        noiseTexture.filterMode = FilterMode.Point;
        GenerateNoise();
    }

    private void GenerateNoise()
    {
        for (int y = 0; y < noiseSize; y++)
        {
            for (int x = 0; x < noiseSize; x++)
            {
                float value = Random.value;
                Color color = new Color(value, value, value, 1f);
                noiseTexture.SetPixel(x, y, color);
            }
        }

        noiseTexture.Apply();
    }

    private void UpdateStatic()
    {
        staticTimer += Time.deltaTime;

        if (staticTimer >= staticRefreshRate)
        {
            GenerateNoise();
            staticTimer = 0f;
        }
    }

    private void CreateScanlineTexture()
    {
        scanlineTexture = new Texture2D(1, scanlineTextureHeight);
        scanlineTexture.filterMode = FilterMode.Point;
        scanlineTexture.wrapMode = TextureWrapMode.Repeat;

        for (int y = 0; y < scanlineTextureHeight; y++)
        {
            bool isDarkLine = y % 4 == 0;
            Color color = isDarkLine ? Color.black : Color.clear;
            scanlineTexture.SetPixel(0, y, color);
        }

        scanlineTexture.Apply();
    }

    private void UpdateScanlines()
    {
        if (scanlineImage == null)
            return;

        Rect uv = scanlineImage.uvRect;
        uv.y += scanlineScrollSpeed * Time.deltaTime;
        scanlineImage.uvRect = uv;
    }

    private void UpdateTimestamp()
    {
        if (timestampText == null)
            return;

        int minutes = Mathf.FloorToInt(gameTimer / 60f);
        int seconds = Mathf.FloorToInt(gameTimer % 60f);
        int milliseconds = Mathf.FloorToInt((gameTimer * 100f) % 100f);

        timestampText.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    private void UpdateREC()
    {
        if (recText == null)
            return;

        float blink = Mathf.PingPong(Time.time, recBlinkSpeed);

        recText.enabled = blink > recBlinkSpeed * 0.3f;
    }
}
