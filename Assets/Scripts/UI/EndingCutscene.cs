using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingCutscene : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string nextSceneName = "MainMenu";

    [Header("Scrolling Text")]
    [SerializeField] private RectTransform scrollingText;
    [SerializeField] private float scrollSpeed = 60f;
    [SerializeField] private float startY = -700f;
    [SerializeField] private float endY = 900f;

    [Header("Input")]
    [SerializeField] private float minimumTimeBeforeSkip = 0.5f;

    private float timer = 0f;
    private bool hasFinished = false;

    private void Start()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (scrollingText != null)
        {
            scrollingText.anchoredPosition = new Vector2(
                scrollingText.anchoredPosition.x,
                startY
            );
        }
    }

    private void Update()
    {
        timer += Time.unscaledDeltaTime;

        if (scrollingText == null)
            return;

        if (!hasFinished)
        {
            scrollingText.anchoredPosition += Vector2.up * scrollSpeed * Time.unscaledDeltaTime;

            if (scrollingText.anchoredPosition.y >= endY)
            {
                hasFinished = true;
                Invoke(nameof(LoadNextScene), 2f);
            }
        }

        if (timer >= minimumTimeBeforeSkip)
        {
            if (Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.Escape) ||
                Input.GetMouseButtonDown(0))
            {
                LoadNextScene();
            }
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
    }
}
