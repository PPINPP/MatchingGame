using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MatchingGame.Gameplay
{
    [Serializable]
    public class SceneName
    {
        public string mainScene;
        public string tutorialScene;
        public string gameplayScene;
        public string smileScene;
        public string minigameScene;
        public string uiTestScene;
        public string dailyScene;
    }

    [DefaultExecutionOrder(-1)]
    public class GameplayResources : SerializedMonoBehaviour
    {
        private static GameplayResources instance;

        public static GameplayResources Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Instantiate(Resources.Load<GameplayResources>("Gameplay/GameplayResources"));
                }
                return instance;
            }
        }

        private void Awake()
        {
            Init();
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
        [SerializeField] Dictionary<CategoryTheme, CardCategoryDataSO> cardCategoryDataDic;
        [SerializeField] Dictionary<CardImgType, Sprite> cardImgDic;
        [SerializeField] Dictionary<CategoryTheme, Sprite> backCardImg;
        public List<SoundEffectSO> SoundEffectList;
        public GameObject soundPrefab;

        #region Header 
        [Space(10)]
        [Header("Gameplay Setting")]
        #endregion
        [SerializeField] PairConfigSO pairConfigData;
        [SerializeField] RandomPatternTranformSO randomPatternTranformData;
        [SerializeField] GameplayProperty gameplayProperty;
        [SerializeField] SceneName sceneNames;

        public PairConfigSO PairConfigData { get => pairConfigData; }
        public RandomPatternTranformSO RandomPatternTranformData { get => randomPatternTranformData; }
        public Dictionary<CategoryTheme, CardCategoryDataSO> CardCategoryDataDic { get => cardCategoryDataDic; }
        public Dictionary<CardImgType, Sprite> CardImgDic { get => cardImgDic; }
        public Dictionary<CategoryTheme, Sprite> BackCardImg { get => backCardImg; }
        public GameplayProperty GameplayProperty { get => gameplayProperty; }
        public SceneName SceneNames { get => sceneNames; }


        public void Init()
        {
            foreach (CardCategoryDataSO cardCategoryData in cardCategoryDataDic.Values)
            {
                cardCategoryData.SetObjValue();
            }
        }
    }
}