using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DifficultyScriptable", order = 2)]
public class DifficultyScriptable : SerializedScriptableObject
{
    public Dictionary<string,StageConfig> stageConfig = new Dictionary<string, StageConfig>();
    [HideInInspector]
    public List<string> easyID = new List<string>();
    [HideInInspector]
    public List<string> normalID = new List<string>();
    [HideInInspector]
    public List<string> hardID = new List<string>();
    [HideInInspector]
    public List<string> advanceID = new List<string>();

    public void SetObjValue()
    {
        foreach (KeyValuePair<string,StageConfig> obj in stageConfig)
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