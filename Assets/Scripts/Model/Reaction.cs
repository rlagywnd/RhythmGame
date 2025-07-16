using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Reaction",menuName ="Reaction")]
public class Reaction : ScriptableObject
{
    [SerializeField]
    protected Dialogue[] dialogues; 
    
    public virtual Dialogue[] GetDialogues()
    {
        return dialogues;
    } 
}

 