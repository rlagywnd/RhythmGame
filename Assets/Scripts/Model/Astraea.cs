using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Astraea : MonoBehaviour
{
    [Header("Core Components")]
    private SpeechBubble speechBubble;
    private Animator anime;
    private GameResultData resultData;
    public bool IsInteracting { get; private set; } = false;

    private Vector3 startPos;

    [Header("Floating Motion")]
    [SerializeField] private float floatAmplitude = 0.5f;   // 위아래 이동 범위
    [SerializeField] private float floatSpeed = 1f;         // 이동 속도

    [Header("Touch Reactions")]
    [SerializeField] private Reaction[] headReactions;
    [SerializeField] private Reaction[] chestReactions;
    [SerializeField] private Reaction[] bodyReactions;
    [SerializeField] private Reaction[] legReactions;

    [Header("Event Reactions")]
    [SerializeField] private Reaction[] selectMusicReactions;
    [SerializeField] private Reaction[] gameEndReactions;

    [Header("Reaction Settings")]
    [Range(0, 1)]
    [SerializeField] private float selectMusicReactionChance;

    [Header("Debug")]
    private bool test = false;

    private void Awake()
    {
        anime = transform.GetComponentInChildren<Animator>();
        speechBubble = GetComponentInChildren<SpeechBubble>();
    }
    private void Start()
    {
        startPos = transform.position;
        resultData = SelectedMusic.Instance.resultData;
        if (resultData != null)
        {
            GameEndReaction();
        }

    }
    private void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        test = Input.GetKey(KeyCode.Space);
    }

    private IEnumerator StartDialogue(Dialogue[] dialogues,float delay = 0)
    {
        if(delay > 0)
        {
            yield return WaitForSecondsCache.Get(delay);
        }
        for (int j = 0; j < dialogues.Length; j++)
        {
            float duration = dialogues[j].duration;
            string animeName = dialogues[j].animeName;
            string text = dialogues[j].text;

            if (j > 0)
            {
                string prevAnimeName = dialogues[j - 1].animeName;
                SetAnime(prevAnimeName, false);
            }
            DialogueStart(text);
            SetAnime(animeName, true);
            yield return WaitForSecondsCache.Get(duration);
        }
        
        yield return speechBubble.Disable();
        IsInteracting = false;

        SetAnime(dialogues[dialogues.Length - 1].animeName, false);
    }
    public void OnCharacterTouched(BodyParts bodyPart)
    {
        if (IsInteracting)
        {
            return;
        }
        PlayReaction(GetReactionForBodyPart(bodyPart));
    }

    private void GameEndReaction()
    {
        var reaction = GetRandomReaction(gameEndReactions);
        var original = reaction.GetDialogues();
        var newDialogues = new Dialogue[original.Length];
        for (int i = 0; i < original.Length; i++)
        {
            newDialogues[i] = new Dialogue
            {
                text = original[i].text.Replace("{score}", $"{resultData.score}")
                                       .Replace("{maxScore}", $"{resultData.maxScore}")
                                       .Replace("{rank}", $"{resultData.rank}"), 
                duration = original[i].duration,
                animeName = original[i].animeName
            };
        }
        PlayReaction(newDialogues,1.5f);
    }

    public void OnSelectMusicReaction(MusicData selectMusic)
    {
        if (test)
        {
            //테스트용 코드
            var origin = selectMusicReactions[0].GetDialogues();
            var dialogues_ = ReplaceTagsInDialogues(origin, selectMusic);

            PlayReaction(dialogues_);
            return;
        }

        float chance = Random.value;
        if(chance > selectMusicReactionChance || IsInteracting)
        {
            return;
        }

        var reaction = GetRandomReaction(selectMusicReactions); 
        var original = reaction.GetDialogues();
        var dialogues = ReplaceTagsInDialogues(original,selectMusic);
         
        PlayReaction(dialogues);
    }
    private Dialogue[] ReplaceTagsInDialogues(Dialogue[] original, MusicData musicData)
    {
        Dialogue[] newDialogues = new Dialogue[original.Length];

        for (int i = 0; i < original.Length; i++)
        {
            newDialogues[i] = new Dialogue
            {
                text = original[i].text.Replace("{score}", $"{musicData.BestScore}"),
                duration = original[i].duration,
                animeName = original[i].animeName
            };
        }

        return newDialogues;
    }

    private void PlayReaction(Dialogue[] dialogues,float delay = 0)
    {
        IsInteracting = true; 
        StartCoroutine(StartDialogue(dialogues,delay));
    }
    private Dialogue[] GetReactionForBodyPart(BodyParts bodyPart)
    {
        Dialogue[] dialogues = null;
        Reaction reaction = null;

        switch (bodyPart)
        {
            case BodyParts.Head:
                reaction = GetRandomReaction(headReactions);
                break;
            case BodyParts.Chest:
                reaction = GetRandomReaction(chestReactions); 
                break;
            case BodyParts.Body:
                reaction = GetRandomReaction(bodyReactions); 
                break;
            case BodyParts.Leg:
                reaction = GetRandomReaction(bodyReactions);
                break; 
        } 

        dialogues = reaction.GetDialogues();
        return dialogues;
    }
    private T GetRandomReaction<T>(T[] array)
    {
        if (array == null || array.Length == 0)
        {
            return default;
        } 

        return array[Random.Range(0, array.Length)];
    } 

    public void DialogueStart(string text)
    { 
        StartCoroutine(speechBubble.StartText(text));
    } 

    public void SetAnime(string animeName, bool trigger)
    { 
        if(animeName == string.Empty)
        {
            return;
        }
        anime.SetBool(animeName, trigger);
    }
}
