using UnityEngine;

[System.Serializable]
public struct MusicInfo
{
    public string name;
    public int maxBlock;
    public int BPM;
    public int offset;
    public Note[] notes;  
}
