using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "MusicData", menuName = "Scriptable Objects/MusicData")]
public class MusicData : ScriptableObject
{
    public AudioClip music; 
    public float highlight;
     
    public int BestScore
    {
        get
        {
            Debug.Log(PlayerPrefs.GetInt(name, 0));
            return PlayerPrefs.GetInt(name, 0);
        }
        set
        {
            var origin = PlayerPrefs.GetInt(name, 0);
            var best = Mathf.Max(value, origin);
            PlayerPrefs.SetInt(name, best); 
        }
    }

    public new string name;
    public int maxBlock;
    public int BPM;
    public int offset;
    public Note[] notes;

    [SerializeField]
    private SettingData setting;
     
    public int GetMaxScore()
    {
        int total = 0;
        int noteCount = NoteCount(this.notes);
        int baseScore = (int)JudgementType.Perfect;

        for (int combo = 1; combo <= noteCount; combo++)
        {
            int bonusScore = Mathf.RoundToInt(baseScore * combo * setting.comboBonus);
            total += baseScore + bonusScore;
        }

        return total;
    }



    public int NoteCount(Note[] notes)
    {
        int count = 0;
        if (notes == null || notes.Length == 0)
        {
            return 0;
        } 

        for (int i = 0; i < notes.Length; i++)
        {
            count++; // 본인 노트 하나 세기
            if (notes[i].notes != null && notes[i].notes.Length > 0)
            {
                count += NoteCount(notes[i].notes); // 자식 노트 재귀적으로 세기
            }
        }
        return count;
    }
}
