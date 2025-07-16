using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSelectUISort : MonoBehaviour
{   
    public RectTransform viewport; // Viewport 할당
    public RectTransform content;  // Content 할당
     
    private void Start()
    {
        
        StartCoroutine(DelayedSort());
    }

    private IEnumerator DelayedSort()
    {
        yield return null; // 한 프레임 딜레이
        Sort();
    }

    public void Sort()
    {
        var visibleItems = GetVisibleObjects();
        int count = visibleItems.Count; 
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        { 
            var uiScript = visibleItems[i].transform.GetChild(0).GetComponent<MusicSelectUI>();
            if (uiScript == null) continue;
            uiScript.itemIndex = i;
            uiScript.itemCount = count; 
        }
    }
     
    private List<GameObject> GetVisibleObjects()
    {
        var result = new List<GameObject>();
        Rect viewportRect = GetWorldRect(viewport);

        foreach (Transform child in content)
        {
            RectTransform childRect = child as RectTransform;
            if (childRect == null)
                continue;

            Rect childWorldRect = GetWorldRect(childRect);
            if (viewportRect.Overlaps(childWorldRect, true))
            {
                result.Add(child.gameObject);
            }
        }
        return result;
    }
     
    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector3 lb = corners[0];
        Vector3 rtCorner = corners[2];
        return new Rect(lb, rtCorner - lb);
    }
}
