using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Animator AN;

    public void LoadScene(int index)
    {
        if (index == 0)
		{
            AppManager.Instance.SaveTempSecond(0);
        }

        StartCoroutine(LoadSceneAsync(index));
    }

    IEnumerator LoadSceneAsync(int index)
    {
        loadingScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        AsyncOperation async = SceneManager.LoadSceneAsync(index);
        while (!async.isDone)
        {
            yield return null;
        }


        AN.Play("LoadingCanvasFadeOut");
        yield return new WaitForSeconds(1);

        loadingScreen.SetActive(false);
    }
}
