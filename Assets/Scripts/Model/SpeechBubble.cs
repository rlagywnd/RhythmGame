using TMPro;
using UnityEngine;
using System.Collections;

public class SpeechBubble : MonoBehaviour
{
    [Header("Bubble Settings")]
    [SerializeField] private float fixedWidth = 500f;
    [SerializeField] private Vector2 padding = new(40f, 20f);
    [SerializeField] private Vector2 bubblePadding = new(20f, 20f);
    [SerializeField] private float duration;

    [Header("References")]
    [SerializeField] private RectTransform bubbleRect;
    [SerializeField] private RectTransform tailRect;

    [Header("Runtime Components")]
    private CanvasGroup group;
    private TextMeshProUGUI tmp;
    private RectTransform textRect;

    [Header("Tail Settings")]
    private Vector2 tailLocalRatio;


    private void Awake()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        textRect = tmp.GetComponent<RectTransform>();
        group = GetComponent<CanvasGroup>();

        tmp.overflowMode = TextOverflowModes.Overflow;
        SaveTailLocalRatio();
    }

    public IEnumerator StartText(string text)
    {
        if (group.alpha > 0)
            yield return Fade(1, 0, duration);

        SetText(text);

        yield return Fade(0, 1, duration);
    }

    public IEnumerator Disable()
    {
        yield return Fade(1, 0, duration);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            float a = Mathf.Lerp(from, to, time / duration);
            group.alpha = a;
            time += Time.deltaTime;
            yield return null;
        }

        group.alpha = to;
    }

    public void SetText(string text)
    {
        if (textRect == null || bubbleRect == null)
            return;


        tmp.text = $"<size=80%> 아스트레아</size><line-height=150%>\n</line-height>{text}";
        tmp.ForceMeshUpdate();

        Vector2 textSize = UpdateTextSize();
        UpdateBubbleSize(textSize + bubblePadding);
        UpdateTailPosition();
    }

    /// <summary> 텍스트 크기 기반으로 textRect 크기 조정 후 반환 </summary>
    private Vector2 UpdateTextSize()
    {
        Vector2 textSize = new Vector2(
            Mathf.Min(fixedWidth, tmp.preferredWidth),
            tmp.textBounds.size.y
        );

        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textSize.x);
        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textSize.y);
        return textSize;
    }

    /// <summary> 말풍선의 크기만 변경 (위치 고정, pivot = 왼쪽) </summary>
    private void UpdateBubbleSize(Vector2 newSize)
    {
        bubbleRect.sizeDelta = newSize;
    }

    /// <summary> 꼬리의 위치를 말풍선 비율에 따라 재배치 </summary>
    private void UpdateTailPosition()
    {
        if (tailRect == null || bubbleRect == null) return;

        Vector2 size = bubbleRect.sizeDelta;
        Vector2 newTailPos = new Vector2(
            size.x * tailLocalRatio.x,
            size.y * tailLocalRatio.y
        );

        tailRect.anchoredPosition = newTailPos;
    }

    /// <summary> 꼬리의 초기 위치 비율 저장 </summary>
    private void SaveTailLocalRatio()
    {
        if (bubbleRect == null || tailRect == null) return;

        Vector2 size = bubbleRect.rect.size;
        Vector2 pos = tailRect.anchoredPosition;

        tailLocalRatio = new Vector2(
            size.x != 0 ? pos.x / size.x : 0f,
            size.y != 0 ? pos.y / size.y : 0f
        );
    }
}
