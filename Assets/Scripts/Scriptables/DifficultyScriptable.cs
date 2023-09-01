using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DifficultyScriptable", order = 2)]
public class DifficultyScriptable : SerializedScriptableObject
{
    [Header("Load Img Config")]
    [SerializeField] string category;
    [SerializeField] string key;
    [SerializeField] int number;
    [Space(5)]
    [Header("Stage Config")]
    public Dictionary<string, StageConfig> stageConfig = new Dictionary<string, StageConfig>();
    [HideInInspector]
    public List<string> easyID = new List<string>();
    [HideInInspector]
    public List<string> normalID = new List<string>();
    [HideInInspector]
    public List<string> hardID = new List<string>();
    [HideInInspector]
    public List<string> advanceID = new List<string>();

    [ContextMenu("Load All Image")]
    public void LoadAllImg()
    {
        for (int i = 1; i < number; i++)
        {
            CreateDic(i);
        }
    }
    [ContextMenu("Load  Image")]
    public void LoadImg()
    {
        CreateDic(number);
    }

    private void CreateDic(int number)
    {
        var Config = new StageConfig();

        string easyPath = $"Assets/Texture/{category}/1_Easy/{key}_E_{number:0000}.png";
        var easySprite = (Sprite)AssetDatabase.LoadAssetAtPath(easyPath, typeof(Sprite));
        Config.easySprite = easySprite;

        string normalPath = $"Assets/Texture/{category}/2_Normal/{key}_N_{number:0000}.png";
        var normalSprite = (Sprite)AssetDatabase.LoadAssetAtPath(normalPath, typeof(Sprite));
        Config.normalSprite = normalSprite;

        string hardPath = $"Assets/Texture/{category}/3_Hard/{key}_H_{number:0000}.png";
        var hardSprite = (Sprite)AssetDatabase.LoadAssetAtPath(hardPath, typeof(Sprite));
        Config.hardSprite = hardSprite;

        string advancePath = $"Assets/Texture/{category}/3_Hard/{key}_A_{number:0000}.png";
        var advanceSprite = (Sprite)AssetDatabase.LoadAssetAtPath(advancePath, typeof(Sprite));
        Config.advanceSprite = advanceSprite;

        stageConfig.Add($"{key}_{number:0000}", Config);
    }

    public void SetObjValue()
    {
        foreach (KeyValuePair<string, StageConfig> obj in stageConfig)
        {
            if (obj.Value.easySprite != null)
            {
                easyID.Add(obj.Key);
                obj.Value.SetEasy();
            }

            if (obj.Value.normalSprite != null)
            {
                normalID.Add(obj.Key);
                obj.Value.SetNormal();

            }

            if (obj.Value.hardSprite != null)
            {
                hardID.Add(obj.Key);
                obj.Value.SetHard();
            }

            if (obj.Value.advanceSprite != null)
            {
                advanceID.Add(obj.Key);
                obj.Value.SetAdvanced();
            }
        }
    }
}

[Serializable]
public class StageConfig
{
    public bool isEasy { get; private set; }
    public bool isNormal { get; private set; }
    public bool isHard { get; private set; }
    public bool isAdvanced { get; private set; }
    public Sprite easySprite;
    public Sprite normalSprite;
    public Sprite hardSprite;
    public Sprite advanceSprite;

    public void SetEasy()
    {
        isEasy = true;
    }

    public void SetNormal()
    {
        isNormal = true;
    }

    public void SetHard()
    {
        isHard = true;
    }

    public void SetAdvanced()
    {
        isAdvanced = true;
    }
}