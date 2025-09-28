using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainScreen;

    [SerializeField] private Slider loadingSlider;
    [SerializeField] private float loadDelay = 1f;
    [SerializeField] private TextMeshProUGUI loadingTxt;

    public void Load(int sceneIndex)
    {
        mainScreen.SetActive(false);
        loadingScreen.SetActive(true);

        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        yield return new WaitForSeconds(loadDelay);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            loadingSlider.value = progress;
            loadingTxt.text = $"Loading...{Mathf.Floor(progress * 100)}%";

            if (asyncOperation.progress >= 1f - 0.001f)
            {
                loadingTxt.text = "Press any key to continue";
                if (Input.anyKeyDown)
                {
                    asyncOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}
