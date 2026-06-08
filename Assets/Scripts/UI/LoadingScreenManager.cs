using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private TMP_Text tipText;

    [SerializeField] private float minimumLoadingTime = 2f;

    private void Start()
    {
        if (tipText != null)
            tipText.text = "Remember to describe the environment to the other player.";

        StartCoroutine(LoadTargetScene());
    }

    private IEnumerator LoadTargetScene()
    {
        string sceneToLoad = LoadingData.SceneToLoad;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("No scene selected to load.");
            yield break;
        }

        float timer = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            timer += Time.deltaTime;

            float loadingProgress = Mathf.Clamp01(operation.progress / 0.9f);
            float timeProgress = Mathf.Clamp01(timer / minimumLoadingTime);

            float displayedProgress = Mathf.Min(loadingProgress, timeProgress);

            if (progressBar != null)
                progressBar.value = displayedProgress;

            if (loadingText != null)
                loadingText.text = "LOADING... " + Mathf.RoundToInt(displayedProgress * 100f) + "%";

            if (operation.progress >= 0.9f && timer >= minimumLoadingTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
