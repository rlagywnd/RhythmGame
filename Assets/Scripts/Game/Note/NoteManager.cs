using System.Collections.Generic; 
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance { get; private set; }

    [Header("일반노트 판정 기준")]
    [SerializeField] private float perfectRange = 0.03f;
    [SerializeField] private float goodRange = 0.07f;
    [SerializeField] private float badRange = 0.12f; 

    [Header("롱노트 판정 기준")]
    [SerializeField] private float perfectHeight = 0.90f;
    [SerializeField] private float goodHeight = 0.8f;
    [SerializeField] private float badHeight = 0.7f;

    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;

    private Queue<NoteObject>[] notes = new Queue<NoteObject>[4];

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < notes.Length; i++)
            notes[i] = new Queue<NoteObject>();
    }

    public void SetNote(NoteObject note, int laneNum)
    {
        notes[laneNum].Enqueue(note);
    }

    void Update()
    {
        RefreshNoteArray();
    }

    private float GetTimingOffset(NoteObject note)
    {
        float musicTime = GameManager.Instance.musicTime;
        float offset = GameManager.Instance.settingData.musicStartDelay;
        float time = note.time - offset - musicTime;
         
        return time;
    }
    


    private void RefreshNoteArray()
    {
        if (!GameManager.Instance.IsStarted)
        {
            return;
        }

        for (int i = 0; i < notes.Length; i++)
        {
            if (notes[i].Count == 0) continue;
            NoteObject note = notes[i].Peek();
            float time = GetTimingOffset(note);
            if (time <= -badRange)
            { 
                note.gameObject.SetActive(false);
                notes[i].Dequeue();
                GameManager.Instance.AddScore(JudgementType.Miss);
            }
        }
    } 
    // 일반 노트 판정(예: 버튼 입력시)
    public void HitNote(int laneNum)
    {  
        if (notes[laneNum].Count == 0)
        {
            return;
        }
        NoteObject note = notes[laneNum].Peek();

        if(note is LongNoteObject lno && !lno.IsLongStartNote())
        {
            float height = lno.GetLongNoteProgress(); 
            if (height >= perfectHeight)
            {
                note.gameObject.SetActive(false);
                notes[laneNum].Dequeue();
                GameManager.Instance.AddScore(JudgementType.Perfect); 
            }
            else if (height >= goodHeight)
            {
                note.gameObject.SetActive(false);
                notes[laneNum].Dequeue();
                GameManager.Instance.AddScore(JudgementType.Good); 

            }
            else if (height >= badHeight)
            {
                note.gameObject.SetActive(false);
                notes[laneNum].Dequeue();
                GameManager.Instance.AddScore(JudgementType.Bad); 

            }
            else
            {
                note.gameObject.SetActive(false);
                notes[laneNum].Dequeue();
                GameManager.Instance.AddScore(JudgementType.Miss);
            }
            return;
        } 
        float time = Mathf.Abs(GetTimingOffset(note));
        // 일반 노트만 판정
        
        if (time <= perfectRange)
        {
            note.gameObject.SetActive(false);
            notes[laneNum].Dequeue();
            GameManager.Instance.AddScore(JudgementType.Perfect); 
        }
        else if (time <= goodRange)
        {
            note.gameObject.SetActive(false);
            notes[laneNum].Dequeue();
            GameManager.Instance.AddScore(JudgementType.Good); 
        }
        else if (time <= badRange)
        {
            note.gameObject.SetActive(false);
            notes[laneNum].Dequeue();
            GameManager.Instance.AddScore(JudgementType.Bad); 
        } 
        audioSource.PlayOneShot(hitSound);
    } 

    public NoteObject GetCurrentNote(int laneNum)
    { 

        if (notes[laneNum].Count == 0)
        {
            return null;

        }
        return notes[laneNum].Peek();
    }
}
