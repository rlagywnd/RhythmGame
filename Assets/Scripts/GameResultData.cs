using System.Collections.Generic;
using UnityEngine;

public class GameResultData
{
    public int score;
    public int combo; 
    public Rank rank;
    public int maxScore;
    public Dictionary<JudgementType, int> judgementCount;
}

public enum Rank
{
    S,
    A,
    B,
    C,
    D
}