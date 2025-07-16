using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections; 

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Image progressBar; 
    [SerializeField] private Material fadeMat;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float start;
    [SerializeField] private float end;
    private void Start()
    {
        StartCoroutine(FadeOut());
    }
    
     
    private IEnumerator FadeOut()
    {
        yield return null;
        float elapsed = 0f;
        float value = start;
        fadeMat.SetFloat("_FadeProgress", start);
        fadeMat.SetFloat("_FadeSharpness", 0.1f);
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            value = Mathf.Lerp(start, end, t);
            fadeMat.SetFloat("_FadeProgress", value);
            yield return null;
        }
        fadeMat.SetFloat("_FadeProgress", end);
        fadeMat.SetFloat("_FadeSharpness", 10);

        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Game");
        operation.allowSceneActivation = false;

        while (progressBar.fillAmount < 1)
        {
            progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, 1f, Time.deltaTime);
            yield return null;
        }
         
        yield return WaitForSecondsCache.Get(1);
        operation.allowSceneActivation = true;
    }

}
