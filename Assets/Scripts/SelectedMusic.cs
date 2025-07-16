using UnityEngine;

public class SelectedMusic : MonoBehaviour
{
    public static SelectedMusic Instance;
    public MusicData selectedMusic;
    public GameResultData resultData;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    } 
}
