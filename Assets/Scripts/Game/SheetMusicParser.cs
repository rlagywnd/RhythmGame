using Unity.VisualScripting;
using UnityEngine;

public class SheetMusicParser : MonoBehaviour
{
    private MusicData selectMusic;
    [SerializeField] private NoteObject normalNotePrefab;
    [SerializeField] private LongNoteObject longNotePrefab;
    [SerializeField] private RectTransform judgeLine;
    [SerializeField] private SettingData settingData;
    [SerializeField] private Transform[] lines;
    [SerializeField] private Canvas canvas;
    private float offset;

    void Start()
    {
        selectMusic = GameManager.Instance.MusicData;
        offset = GameManager.Instance.settingData.musicStartDelay;
        GenerateNotes(selectMusic);
    } 
    private MusicInfo SheetParse(TextAsset sheetMusicFile)
    {
        string text = sheetMusicFile.text;
        MusicInfo musicInfo = JsonUtility.FromJson<MusicInfo>(text);
        return musicInfo;
    }

    private void GenerateNotes(MusicData musicInfo)
    {
        int bpm = musicInfo.BPM;
        GenerateNotesRecursive(musicInfo.notes, bpm);
        GameManager.Instance.GameStart();
    }

    // 재귀 노트 생성 (롱노트까지)
    private void GenerateNotesRecursive(Note[] notes, int bpm)
    {
        foreach (var note in notes)
        {
            GenerateNoteRecursive(note, bpm);
        }
    }

    private NoteObject GenerateNoteRecursive(Note note, int bpm)
    {
        NoteObject noteObj = GenerateNote(note, bpm);

        // 롱노트(자식) 처리
        if (note.notes != null && note.notes.Length > 0)
        {
            foreach (var child in note.notes)
            {
                NoteObject startObj = noteObj; // 부모가 Start
                NoteObject endObj = GenerateNoteRecursive(child, bpm); // 자식이 End

                // 롱노트 연결(type==2)만 연결 (LongNoteObject에만)
                if (note.type == 2 && child.type == 2 &&
                    startObj is LongNoteObject start && endObj is LongNoteObject end)
                {
                    // **End에서 Start로 연결!**
                    end.SetLongNoteStart(start);
                }
            }
        }
        return noteObj;
    }


    // 프리팹 분기: 일반/롱노트
    private NoteObject GenerateNote(Note note, int bpm)
    {
        float secondsPerBeat = 60.0f / bpm;
        float time = (secondsPerBeat / note.LPB) * note.num;

        time += offset;
        NoteObject note_;
        if (note.type == 2)
        {
            note_ = Instantiate(longNotePrefab);
            LongNoteObject lnote = note_ as LongNoteObject;
            lnote.judgeLineWorldPos = GetUIPos(judgeLine, canvas);
        }
        else
        {
            note_ = Instantiate(normalNotePrefab);
        }  

        Vector2 notePos = CalculateNotePos(note, bpm, note_);
        note_.transform.position = notePos;
        note_.transform.SetParent(GameManager.Instance.transform);
        NoteManager.Instance.SetNote(note_, note.block);
        note_.Init(note, time); // 타이밍 정확히 전달
        return note_;
    }

    private Vector2 CalculateNotePos(Note note, int bpm, NoteObject noteOjb)
    {
        float secondsPerBeat = 60.0f / bpm;
        float time = (secondsPerBeat / note.LPB) * note.num + offset;
        float judgeLinePosY = GetUIPos(judgeLine, canvas).y;
        float yPos = time * settingData.noteSpeed + judgeLinePosY;
        noteOjb.time = time;
        float x = lines[note.block].position.x;
        return new Vector2(x, yPos);
    }

    private Vector3 GetUIPos(RectTransform uiRect, Canvas canvas)
    {
        Camera cam = canvas.worldCamera;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, uiRect.position);
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, cam.nearClipPlane + 1f));
        worldPos.z = 0f;
        return worldPos;
    }
}
