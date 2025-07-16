using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    private GameResultData gameResultData;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private CanvasGroup rankImage;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float scoreDuration; 
    [SerializeField] private float rankDuration; 

    private void Start()
    {
        gameResultData = SelectedMusic.Instance.resultData;
        int perCount = gameResultData.judgementCount[JudgementType.Perfect];
        int goodCount = gameResultData.judgementCount[JudgementType.Good];
        int badCount = gameResultData.judgementCount[JudgementType.Bad];
        int missCount = gameResultData.judgementCount[JudgementType.Miss];

        countText.text = $"Perfect: {perCount}\tGood: {goodCount}\nBad: {badCount}\t\tMiss: {missCount}";
        comboText.text = $"{gameResultData.combo}";

        
        StartCoroutine(Fade());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        { 
            SceneManager.LoadScene("Lobby");
        }
    }
    IEnumerator ScoreText()
    {
        float elapsed = 0;
        int score_ = 0; 
        while (elapsed < scoreDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration); // 0 ~ 1 사이
            score_ = (int)Mathf.Lerp(0, gameResultData.score, t);
            scoreText.text = score_.ToString();
            yield return null;
        }
        scoreText.text = gameResultData.score.ToString();

        StartCoroutine(RankFade());
    }
    IEnumerator Fade()
    {
        float elapsed = 0;
        float alpha = group.alpha;
        rankText.text = gameResultData.rank.ToString();
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration); // 0 ~ 1 사이
            alpha = Mathf.Lerp(0, 1, t);
            group.alpha = alpha; 
            yield return null;
        }
        group.alpha = 1;

        StartCoroutine(ScoreText());
    }
    IEnumerator RankFade()
    {
        float elapsed = 0;
        float alpha = rankImage.alpha;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration); // 0 ~ 1 사이
            alpha = Mathf.Lerp(0, 1, t);
            rankImage.alpha = alpha;
            yield return null;
        }
        rankImage.alpha = 1;
    }
}
