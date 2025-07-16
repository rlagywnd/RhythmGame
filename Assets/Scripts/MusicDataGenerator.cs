#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;

public class MusicDataWindow : EditorWindow
{
    private TextAsset jsonFile;
    private AudioClip audioClip; 
    private float highlight;

    [MenuItem("Tools/Music Data Generator")]
    public static void ShowWindow()
    {
        GetWindow<MusicDataWindow>("Music Data Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label(" Music JSON to ScriptableObject", EditorStyles.boldLabel);

        jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);
        audioClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", audioClip, typeof(AudioClip), false);  
        highlight = EditorGUILayout.FloatField("Highlight Time", highlight);  

        if (GUILayout.Button("Generate"))
        {
            if (jsonFile == null)
            {
                EditorUtility.DisplayDialog("Error", "Please drag and drop a JSON file.", "OK");
                return;
            }
            GenerateMusicData(jsonFile, audioClip);  
        }
    }

    private void GenerateMusicData(TextAsset json, AudioClip clip)
    {
        string jsonText = json.text;
        MusicInfo jsonData = JsonUtility.FromJson<MusicInfo>(jsonText);

        MusicData asset = ScriptableObject.CreateInstance<MusicData>();
        asset.music = clip; 
        asset.highlight = highlight;
        asset.name = jsonData.name;
        asset.BPM = jsonData.BPM;
        asset.maxBlock = jsonData.maxBlock;
        asset.offset = jsonData.offset;
        asset.notes = jsonData.notes;

        string folderPath = "Assets/Resources/MusicData";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string assetPath = Path.Combine(folderPath, jsonData.name + ".asset");
        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Success", $"MusicData created at {assetPath}", "OK");
    }
}
#endif