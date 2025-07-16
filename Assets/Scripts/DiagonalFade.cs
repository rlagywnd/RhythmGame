using UnityEngine;
using System.Collections;

public class DiagonalFade : MonoBehaviour
{
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
    }

}
