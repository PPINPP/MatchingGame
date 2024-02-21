using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PairScriptable", order = 1)]
public class PairConfigSO : ScriptableObject
{
    public List<PairConfig> pairConfigs = new List<PairConfig>();
}

[Serializable]
public class PairConfig
{
    public PairType pairType;
    public Vector2LayOut cellSize;
    public Vector2LayOut spacing;
    public int ConstraintRow;
    public int roundShuffle;
}

[Serializable]
public class Vector2LayOut
{
    public int x;
    public int y;
}

public enum PairType
{
    TWO = 2,
    THREE = 3,
    FOUR = 4,
    SIX = 6,
    EIGHT = 8,
}