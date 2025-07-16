using UnityEngine;

public class BodyPart : MonoBehaviour
{
    private Astraea astraea;

    [SerializeField]
    private BodyParts part;
     
    void Awake()
    {
        astraea = GetTopMostParent(gameObject).GetComponent<Astraea>(); 
    }

    private void OnMouseDown()
    {  
        astraea.OnCharacterTouched(part);
    }
    private GameObject GetTopMostParent(GameObject obj)
    {
        Transform current = obj.transform;
        while (current.parent != null)
        {
            current = current.parent;
        }
        return current.gameObject;
    }
}
