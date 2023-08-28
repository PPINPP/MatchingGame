using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayResources : SerializedMonoBehaviour
{
    private static GameplayResources instance;

    public static GameplayResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameplayResources>("GameplayResources");
            }
            return instance;
        }
    }

    public void Init()
    {
        foreach(DifficultyScriptable difficultyScriptable in gameplayDifficultySODict.Values)
        {
            difficultyScriptable.SetObjValue();
        }
    }

    //public void ReadJson()
    //{
    //    var itemResource = Resources.Load<TextAsset>("PowerupItemResources");
    //    var itemData = JsonConvert.DeserializeObject<Dictionary<string, object>>(itemResource.ToString());
    //    powerUpItemData.coinData = JsonConvert.DeserializeObject<Dictionary<string, Coin>>(itemData["coinData"].ToString());
    //}

    #region Header 
    [Space(10)]
    [Header("Gameplay Resource")]
    #endregion
    [SerializeField] PairScriptable gameplayPairSO;
    [SerializeField] Dictionary<string,DifficultyScriptable> gameplayDifficultySODict;

    public PairScriptable GameplayPairSO { get => gameplayPairSO; }
    public Dictionary<string, DifficultyScriptable> GameplayDifficultySODict { get => gameplayDifficultySODict; }
}
