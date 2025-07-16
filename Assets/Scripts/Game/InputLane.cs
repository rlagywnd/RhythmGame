using UnityEngine;

public class InputLane : MonoBehaviour
{
    private KeyCode[] keyCodes =
    {
        KeyCode.D,
        KeyCode.F,
        KeyCode.J,
        KeyCode.K
    };

    [SerializeField]
    private int laneNum;

    [SerializeField]
    private GameObject keyDownImage;

    // 롱노트 홀드 감지용
    private bool isHoldingLongNote = false;

    void Update()
    {


        KeyCode keyCode = keyCodes[laneNum];
        bool isKeyDown = Input.GetKey(keyCode);
        bool isKeyDownDown = Input.GetKeyDown(keyCode);
        bool isKeyUp = Input.GetKeyUp(keyCode);

        // 1. 키 눌림 이펙트
        keyDownImage.gameObject.SetActive(isKeyDown);

        if (!GameManager.Instance.IsStarted)
        {
            return;
        }
        // 2. 키를 막 눌렀을 때 (HitNote 시도)
        if (isKeyDownDown)
        {
            var note = NoteManager.Instance.GetCurrentNote(laneNum);

            if (note is LongNoteObject lno)
            {
                isHoldingLongNote = true;
                lno.isHolding = true;
            }
            NoteManager.Instance.HitNote(laneNum); // 일반 노트 판정
        }

        // 3. 키를 계속 누르고 있을 때 (롱노트 유지 중)
        if (isHoldingLongNote && isKeyDown)
        {
            // (원한다면) 롱노트 바 유지 이펙트 등
            var note = NoteManager.Instance.GetCurrentNote(laneNum);
            if (note is LongNoteObject lno)
            {
                lno.isHolding = true; 
            } 
        }

        // 4. 키를 뗐을 때 (롱노트 End 판정)
        if (isHoldingLongNote && isKeyUp)
        {
            var note = NoteManager.Instance.GetCurrentNote(laneNum);
            if(note is LongNoteObject lno)
            {
                lno.isHolding = false;
            }
            NoteManager.Instance.HitNote(laneNum); // 롱노트 End 판정
            isHoldingLongNote = false;
        }
    }
}
