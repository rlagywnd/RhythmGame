using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public float time;
    public int type;

    public virtual void Init(Note note, float time)
    {
        this.time = time;
        this.type = note.type;
        // type==1(�Ϲ�)�� �߰� ���� (��: sprite)
    }
}
