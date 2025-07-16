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

    // �ճ�Ʈ Ȧ�� ������
    private bool isHoldingLongNote = false;

    void Update()
    {


        KeyCode keyCode = keyCodes[laneNum];
        bool isKeyDown = Input.GetKey(keyCode);
        bool isKeyDownDown = Input.GetKeyDown(keyCode);
        bool isKeyUp = Input.GetKeyUp(keyCode);

        // 1. Ű ���� ����Ʈ
        keyDownImage.gameObject.SetActive(isKeyDown);

        if (!GameManager.Instance.IsStarted)
        {
            return;
        }
        // 2. Ű�� �� ������ �� (HitNote �õ�)
        if (isKeyDownDown)
        {
            var note = NoteManager.Instance.GetCurrentNote(laneNum);

            if (note is LongNoteObject lno)
            {
                isHoldingLongNote = true;
                lno.isHolding = true;
            }
            NoteManager.Instance.HitNote(laneNum); // �Ϲ� ��Ʈ ����
        }

        // 3. Ű�� ��� ������ ���� �� (�ճ�Ʈ ���� ��)
        if (isHoldingLongNote && isKeyDown)
        {
            // (���Ѵٸ�) �ճ�Ʈ �� ���� ����Ʈ ��
            var note = NoteManager.Instance.GetCurrentNote(laneNum);
            if (note is LongNoteObject lno)
            {
                lno.isHolding = true; 
            } 
        }

        // 4. Ű�� ���� �� (�ճ�Ʈ End ����)
        if (isHoldingLongNote && isKeyUp)
        {
            var note = NoteManager.Instance.GetCurrentNote(laneNum);
            if(note is LongNoteObject lno)
            {
                lno.isHolding = false;
            }
            NoteManager.Instance.HitNote(laneNum); // �ճ�Ʈ End ����
            isHoldingLongNote = false;
        }
    }
}
