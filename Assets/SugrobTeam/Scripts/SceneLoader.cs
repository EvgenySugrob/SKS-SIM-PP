using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [SerializeField]private IDisableAfterLoading _controlerUI;

    private void Start()
    {
        _controlerUI = GetComponent<IDisableAfterLoading>();
    }

    public void OnStartGame(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // Обновить прогресс-бар
            //if (progressBar != null)
            //    progressBar.value = progress;

            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
        _controlerUI.DisableAfterLoading();
        Destroy(gameObject);
    }
}
