using UnityEngine;

[CreateAssetMenu(fileName = "SettingData", menuName = "Scriptable Objects/SettingData")]
public class SettingData : ScriptableObject
{
    public float noteSpeed = 5;
    public float comboBonus = 0.02f;
    public float musicStartDelay = 3;
}
