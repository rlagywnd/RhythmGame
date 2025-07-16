using UnityEngine;

[CreateAssetMenu(fileName = "ConditionalReaction", menuName = "Reaction/Conditional")]
public class ConditionalReaction : Reaction
{
    [SerializeField, Range(0f, 1f)]
    protected float threshold = 0.9f; 

    [SerializeField]
    protected Dialogue[] normalDialogues;

    public override Dialogue[] GetDialogues()
    {
        //������ ���������� �ش� ���� �ְ� ������, �ش� �������� ������ �ִ� �ִ� ������ 
        //threshold�������� ���ٸ� ���������� ���(Dialogue)��,
        //���ٸ� ����� ����(normalDialogues)�� ��ȯ
        MusicData musicData = SelectedMusic.Instance.selectedMusic;

        if(musicData.BestScore == 0)
        {
            Dialogue[] arr =
            {
                new Dialogue()
                {
                    animeName = "",
                    duration = 2,
                    text = "���ǵ� ���Ѱ� ������ �غ��� �׷�?"
                }
            };
            return arr;
        }

        float rate = (float)musicData.BestScore / musicData.GetMaxScore(); 
        if (rate >= threshold)
        {
            return dialogues;
        }
        else
        {
            return normalDialogues;
        }
    }
}
