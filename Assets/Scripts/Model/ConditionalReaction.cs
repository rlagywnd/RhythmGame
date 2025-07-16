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
        //뮤직을 선택했을떄 해당 곡의 최고 점수가, 해당 뮤직에서 받을수 있는 최대 점수의 
        //threshold비율보다 높다면 데레데레한 대사(Dialogue)를,
        //낮다면 비웃는 듯한(normalDialogues)를 반환
        MusicData musicData = SelectedMusic.Instance.selectedMusic;

        if(musicData.BestScore == 0)
        {
            Dialogue[] arr =
            {
                new Dialogue()
                {
                    animeName = "",
                    duration = 2,
                    text = "한판도 안한거 같은데 해보지 그래?"
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
