using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public MusicData MusicData { get; private set; }

    [Header("UI Components")]
    [SerializeField] private Image lifeUI;
    [SerializeField] private Text comboText;
    [SerializeField] private Text scoreText;
    [SerializeField] private CanvasGroup textGroup;
    [SerializeField] private CanvasGroup noteLane;

    [Header("UI Settings")]
    [SerializeField] private float fadeDuration;

    [Header("Gameplay Values")]
    [SerializeField] private int life;
    [SerializeField] private int damage;

    [Header("Runtime Status")]
    private int combo;
    private int score;
    private int noteCount;
    private int hitNoteCount = 0;
    private int currentLife;

    [Header("Audio")]
    private AudioSource music;
    public float musicTime { get; private set; }

    [Header("Judgement")]
    private Dictionary<JudgementType, int> judgementCount = new();

    [Header("Settings")]
    public SettingData settingData;

    [Header("State Flags")]
    public bool IsStarted { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
        music = GetComponent<AudioSource>();
    }
    void Start()
    {
        MusicData = SelectedMusic.Instance.selectedMusic;
        noteCount = MusicData.NoteCount(MusicData.notes);
        music.clip = MusicData.music;
        currentLife = life;
        lifeUI.fillAmount = (float)currentLife / life;

        judgementCount.Add(JudgementType.Perfect, 0);
        judgementCount.Add(JudgementType.Good, 0);
        judgementCount.Add(JudgementType.Bad, 0);
        judgementCount.Add(JudgementType.Miss, 0);
    }
    private void Update()
    {
        if (!IsStarted)
        {
            return;
        }
        musicTime = music.time;
        Vector2 pos = settingData.noteSpeed * Time.deltaTime * Vector2.down;
        transform.Translate(pos);

        if (hitNoteCount == noteCount/* || currentLife <= 0*/)
        {
            IsStarted = false;
            StartCoroutine(GameEnd());
        } 
    }
    private IEnumerator GameEnd()
    {
        yield return WaitForSecondsCache.Get(1);

        float elapsed = 0;
        float alpha = noteLane.alpha;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration); // 0 ~ 1 사이
            alpha = Mathf.Lerp(1, 0, t);
            noteLane.alpha = alpha;
            textGroup.alpha = alpha;
            yield return null;
        }
        noteLane.alpha = 0;
        textGroup.alpha = 0;

        int maxScore = MusicData.GetMaxScore();
        SelectedMusic.Instance.resultData = new GameResultData()
        {
            maxScore = maxScore,
            combo = this.combo,
            score = this.score,
            judgementCount = this.judgementCount,
            rank = GetRank(this.score, maxScore),
        }; 
        SelectedMusic.Instance.selectedMusic.BestScore = this.score; 

        
        SceneManager.LoadScene("Result");
    } 
    Rank GetRank(int currentScore,int maxScore)
    {
        float rate = (float)currentScore / maxScore;

        if(rate <= 0.2f)
        {
            return Rank.D;
        }
        else if(rate <= 0.4f)
        {
            return Rank.C; 
        }
        else if(rate <= 0.6f)
        {
            return Rank.B;
        }
        else if(rate <= 0.8f)
        {
            return Rank.A;
        }
        else
        {
            return Rank.S;
        }
    }
    public void AddScore(JudgementType type)
    {
        hitNoteCount++;
        judgementCount[type]++; 
        if (type == JudgementType.Miss)
        {
            GetDamage();
            return;
        }
        combo++;
        comboText.text = combo.ToString();

        score += GetScore(type);
        scoreText.text = score.ToString(); 
    }
    
    private int GetScore(JudgementType type)
    { 
        float comboBonus = 1f + (combo * settingData.comboBonus); // 콤보 1마다 +1% 보너스
        float judgeMultiplier = 0;

        switch (type)
        {
            case JudgementType.Perfect: judgeMultiplier = 1f; break;
            case JudgementType.Good: judgeMultiplier = 0.7f; break;
            case JudgementType.Bad: judgeMultiplier = 0.3f; break; 
        }
        int baseScore = Mathf.RoundToInt((int)type * judgeMultiplier);
        int bonusScore = Mathf.RoundToInt((int)type * combo * settingData.comboBonus);

        return baseScore + bonusScore;
    }
    public void GameStart()
    {
        
        StartCoroutine(MusicStart());
    }
    IEnumerator MusicStart()
    {
        IsStarted = true;
        yield return WaitForSecondsCache.Get(settingData.musicStartDelay);
        music.Play();
    }
    private void GetDamage()
    {
        combo = 0;
        comboText.text = combo.ToString();

        currentLife -= damage;
        lifeUI.fillAmount = (float)currentLife / life;
    }
}
public enum JudgementType
{
    Perfect = 300,
    Good = 100,
    Bad = 50,
    Miss
}