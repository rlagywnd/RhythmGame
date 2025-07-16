using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using NUnit.Framework.Internal;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [Header("Result & Selection")]
    public static GameResultData resultData;
    public MusicSelectUI SelectedMusicUI { get; private set; }
    //public MusicData SelectedMusic { get; private set; }
    private SelectedMusic sm;

    [Header("Music Player")]
    private LobbyMusicPlayer player;

    [Header("Model")]
    [SerializeField] private Astraea astraea;

    [Header("UI Panel")]
    [SerializeField] private RectTransform musicPanel;
    [SerializeField] private float musicPanelMoveDuration;
    [SerializeField] private float originalXPos;
    [SerializeField] private float backXPos;

    [Header("Stars")]
    [SerializeField] private Star[] stars;
    private Vector3[] baseScales; // 각 별의 원래 크기 저장
    private Coroutine bounceStar;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration;

    [Header("State Flags")]
    private bool init = false;

    void Awake()
    {
        Instance = this; 
    }

    private void Start()
    {
        player = LobbyMusicPlayer.Instance;
        sm = SelectedMusic.Instance;
        baseScales = new Vector3[stars.Length];
        
        for (int i = 0; i < stars.Length; i++)
        {
            baseScales[i] = stars[i].transform.localScale;
        }
        player.onAudioPlayStart += Bounce;
        StartCoroutine(Init());   
    }

    public void SelectMusic(MusicSelectUI ui, string musicName)
    {
        if (!init)
        {
            return;
        } 
        if (sm.selectedMusic != null)
        {
            //음악파일은 null이 아닌데 선택한 UI가 null이면 게임 끝나고 로비로 돌아왔다는뜻
            if (SelectedMusicUI != null)
            {
                SelectedMusicUI.DeSelect();
                StopCoroutine(bounceStar);
                AllStop();
            } 
            Resources.UnloadAsset(sm.selectedMusic); 
        }
        SelectedMusicUI = ui;
        sm.selectedMusic = Resources.Load<MusicData>($"MusicData/{musicName}");
         
        player.ChangeMusic(sm.selectedMusic.music,sm.selectedMusic.highlight);
         
        astraea.OnSelectMusicReaction(sm.selectedMusic);
    }
    private void UIDeSelect()
    {
        SelectedMusicUI.DeSelect();
    }

    private void Bounce()
    {
        bounceStar = StartCoroutine(StarBounce());
    }
    public void GameStart()
    { 
        StartCoroutine(GameStart_());
    }

    IEnumerator Init()
    { 
        yield return MoveMusicPanel(true);
        yield return StarsFade(false);
        yield return AstraeaFade(false);
        init = true;

        if (SelectedMusic.Instance.resultData != null)
        {
            var uiArr = FindObjectsOfType<MusicSelectUI>();
            for (int i = 0; i < uiArr.Length; i++)
            {
                MusicSelectUI ui = uiArr[i];
                ui.Select(SelectedMusic.Instance.selectedMusic.name);
            }
        }
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("hifumi-daisuki", 13058);
    }
    IEnumerator GameStart_()
    {
        yield return AstraeaFade(true);
        yield return StarsFade(true);
        yield return MoveMusicPanel(false);
        yield return player.StopMusic();
        SceneManager.LoadScene("Loading");
    }

    IEnumerator MoveMusicPanel(bool start_)
    {
        float elapsed = 0f;
        float start = start_ ? backXPos : originalXPos;
        float end = start_ ? originalXPos : backXPos;
        Vector3 pos = musicPanel.anchoredPosition; 
        while (elapsed < musicPanelMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / musicPanelMoveDuration); // 0 ~ 1 사이
            pos.x = Mathf.Lerp(start, end, t); 
            musicPanel.anchoredPosition = pos;
            yield return null;
        }
        pos.x = end;
        musicPanel.anchoredPosition = pos;
    }
    IEnumerator AstraeaFade(bool fadeOut)
    {
        float elapsed = 0f;
        float start = fadeOut ? 1 : 0;
        float end = fadeOut ? 0 : 1;
        SpriteRenderer sr = astraea.GetComponentInChildren<SpriteRenderer>();
        Color color = sr.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration); // 0 ~ 1 사이
            color.a = Mathf.Lerp(start, end, t);
            sr.color = color;
            yield return null;
        }
        color.a = end;
        sr.color = color;
    }
    IEnumerator StarsFade(bool fadeOut)
    {
        float elapsed = 0f;
        float start = fadeOut ? 1 : 0;
        float end = fadeOut ? 0 : 1;

        Color[] originalColors = new Color[stars.Length];
        SpriteRenderer[] sprites = new SpriteRenderer[stars.Length];

        for (int i = 0; i < stars.Length; i++)
        {
            sprites[i] = stars[i].GetComponent<SpriteRenderer>();
            originalColors[i] = sprites[i].color; 
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float alpha = Mathf.Lerp(start, end, t);  // FadeOut

            for (int i = 0; i < sprites.Length; i++)
            {
                Color col = originalColors[i];
                col.a = alpha;
                sprites[i].color = col;
            }

            yield return null;
        }

        // 최종 알파 확실하게 0으로 설정
        for (int i = 0; i < sprites.Length; i++)
        {
            Color col = sprites[i].color;
            col.a = end;
            sprites[i].color = col;
        }
    }

    float GetBeatProgressPercent(float time, float bpm)
    {
        float secondsPerBeat = 60f / bpm;
        float timeInCurrentBeat = time % secondsPerBeat;
        float progress = (timeInCurrentBeat / secondsPerBeat);
        return progress;
    } 

    IEnumerator StarBounce()
    {  
        float bpm = sm.selectedMusic.BPM;
        float beatTime = 60f / bpm;
        float highlight = sm.selectedMusic.highlight;  
        float percent = GetBeatProgressPercent(highlight, bpm);
         
        float time = beatTime * (1f - percent); 
        yield return WaitForSecondsCache.Get(time);
        BounceAllStars();

        while (true)
        {
            yield return WaitForSecondsCache.Get(beatTime);
            BounceAllStars();
        }
    }

    private void AllStop()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].Stop(); 
        }
    }

    void BounceAllStars()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].TriggerBounce(); 
        }
    } 
}
