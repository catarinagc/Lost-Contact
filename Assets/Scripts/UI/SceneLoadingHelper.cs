using UnityEngine.SceneManagement;

public static class SceneLoadingHelper
{
    public static void LoadSceneWithLoadingScreen(string targetScene)
    {
        LoadingData.SceneToLoad = targetScene;
        SceneManager.LoadScene("LoadingScreen");
    }
}