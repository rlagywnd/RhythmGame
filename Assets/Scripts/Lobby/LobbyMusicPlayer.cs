using System.Collections;
using UnityEngine;

public delegate void OnAudioPlayStart();

public class LobbyMusicPlayer : MonoBehaviour
{
    public static LobbyMusicPlayer Instance { get; private set; }
    public OnAudioPlayStart onAudioPlayStart;

    [SerializeField]
    private float duration;

    private AudioSource musicPlayer;
    private float currentVolume;

    void Awake()
    {
        Instance = this;
        musicPlayer = GetComponent<AudioSource>();
    }

    private void Start()
    {
        currentVolume = musicPlayer.volume;
        musicPlayer.volume = 0;
    }
    public float GetTime()
    {
        return musicPlayer.time;
    }

    public void ChangeMusic(AudioClip clip,float time)
    {
        StartCoroutine(ChangeMusic_(clip, time));
    } 
    public void PlayMusic(AudioClip clip,float time)
    {
        StartCoroutine(PlayMusic_(clip, time));
    }

    private IEnumerator ChangeMusic_(AudioClip clip, float time)
    {
        yield return FadeSound(currentVolume, 0);

        musicPlayer.Stop();
        musicPlayer.clip = clip;
        musicPlayer.Play();
        onAudioPlayStart();
        musicPlayer.time = time;

        yield return FadeSound(0, currentVolume);
    }
    private IEnumerator PlayMusic_(AudioClip clip, float time)
    {
        musicPlayer.clip = clip;
        musicPlayer.time = time;
        musicPlayer.Play();
        yield return FadeSound(0, currentVolume);
        
    }
    public IEnumerator StopMusic()
    {
        yield return FadeSound(musicPlayer.volume, 0);
        musicPlayer.clip = null;
        musicPlayer.Stop();
    }
    public bool IsPlaying()
    {
        return musicPlayer != null && musicPlayer.isPlaying;
    }
    private IEnumerator FadeSound(float start,float end)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            musicPlayer.volume = Mathf.Lerp(start, end, t); 
            yield return null;
        }
        musicPlayer.volume = end;
    }
}
