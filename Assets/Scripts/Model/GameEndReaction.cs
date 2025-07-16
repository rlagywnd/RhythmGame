using UnityEngine;

[CreateAssetMenu(fileName = "GameEndReaction", menuName = "Reaction/GameEndReaction")]
public class GameEndReaction : ConditionalReaction
{
    public override Dialogue[] GetDialogues()
    {
        GameResultData musicData = SelectedMusic.Instance.resultData;
        float rate = (float)musicData.score / musicData.maxScore;
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