using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LongNoteObject : NoteObject
{
    public LongNoteObject startPartner;
    private LineRenderer line;
    private SpriteRenderer sprite;

    // ������(Transform/RectTransform)�� �ܺο��� �޾Ƽ� �Ҵ�
    [HideInInspector] public Vector3 judgeLineWorldPos;
    [HideInInspector] public bool isHolding = false; // Ȧ�� ����
    private float originalLength;

    public override void Init(Note note, float time)
    {
        base.Init(note, time);
        line = GetComponent<LineRenderer>();
        sprite = GetComponent<SpriteRenderer>();
        if (line != null) line.enabled = false;
    }

    public void SetLongNoteStart(LongNoteObject startObj)
    {
        startPartner = startObj;
        if (startPartner != null && line != null)
        {
            line.enabled = true;
            line.positionCount = 2;
            line.SetPosition(0, startPartner.transform.position);
            line.SetPosition(1, transform.position);
            originalLength = Mathf.Abs(transform.position.y - startPartner.transform.position.y);
             
        }
    }

    public float GetLongNoteProgress()
    {
        if (line == null)
        {
            return -1;
        }
        float yPos = line.GetPosition(0).y;
        float yPos1 = line.GetPosition(1).y;
        return 1 - (Mathf.Abs(yPos1 - yPos) / originalLength);
    }
    public bool IsLongStartNote()
    {
        return !line.enabled;
    }

    private void Update()
    {
        if (startPartner != null && line != null && line.enabled)
        {
            line.SetPosition(1, transform.position);
            
            // �� Ȧ�� ���̸� ������ ������, �ƴϸ� End ��Ʈ ��ġ
            if (isHolding)
            {
                Vector2 pos = judgeLineWorldPos;
                pos.x = transform.position.x;
                line.SetPosition(0, pos);
            }
            else
            {
                line.SetPosition(0, startPartner.transform.position);
            } 
        }
    }
     
}
