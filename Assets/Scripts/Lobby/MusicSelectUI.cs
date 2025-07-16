using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MusicSelectUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Music Info")]
    [SerializeField] private string musicName;

    [Header("External Values")]
    [HideInInspector] public int itemIndex; // 외부에서 할당
    [HideInInspector] public int itemCount; // 외부에서 할당

    [Header("Size Settings")]
    private float minWidth = 300f;
    private float maxWidth = 400f;
    private float baseHeight;
    private float heightExpand = 40;
    private float hoverExpand = 60f;

    [Header("Layout & Position")]
    private RectTransform parentPos;
    private RectTransform rect;

    [Header("UI State")]
    private bool isClicked = false;
    private bool isHovered = false;

    [Header("Animation")]
    private float lerpSpeed = 10f;



    void Awake()
    {
        rect = GetComponent<RectTransform>();
        parentPos = transform.parent.GetComponent<RectTransform>();

        baseHeight = parentPos.sizeDelta.y;

    }


    void Update()
    {
        float baseWidth = CalculateItemWidth(itemIndex, itemCount, minWidth, maxWidth);
        float targetWidth = baseWidth + (isHovered ? hoverExpand : 0f);
        Vector2 size = rect.sizeDelta;

        size.x = Mathf.Lerp(size.x, targetWidth, Time.deltaTime * lerpSpeed);
        rect.sizeDelta = size;

        float targetHeight = baseHeight + (isHovered ? heightExpand : 0f);
        Vector2 newHeight = parentPos.sizeDelta;

        newHeight.y = Mathf.Lerp(newHeight.y, targetHeight, Time.deltaTime * lerpSpeed);
        parentPos.sizeDelta = newHeight;
        if (isHovered)
        {
            parentPos.parent.GetComponent<VerticalLayoutGroup>().enabled = false;
            parentPos.parent.GetComponent<VerticalLayoutGroup>().enabled = true;
        }


    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (LobbyManager.Instance.SelectedMusicUI == this)
        {
            return;
        }
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (LobbyManager.Instance.SelectedMusicUI == this)
        {
            return;
        }
        isHovered = false;
    }

    float CalculateItemWidth(int index, int count, float minWidth, float maxWidth)
    {
        if (count <= 1) return maxWidth;
        int center = (count - 1) / 2;
        float distance = Mathf.Abs(index - center);
        float t = 1f - (distance / (float)center);
        t = Mathf.Clamp01(t);
        return Mathf.Lerp(minWidth, maxWidth, t);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (LobbyManager.Instance.SelectedMusicUI == this)
        {
            LobbyManager.Instance.GameStart();
            return;
        }

        Select();
    }

    public void Select(string musicName)
    { 
        if(musicName == this.musicName)
        { 
            Select();
        }
    }
    private void Select()
    {
        isHovered = true;
        LobbyManager.Instance.SelectMusic(this, musicName);
    }
    public void DeSelect()
    {
        isHovered = false;
    }
}
