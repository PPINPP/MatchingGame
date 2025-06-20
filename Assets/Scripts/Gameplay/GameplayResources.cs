using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchingGame.Gameplay
{
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
        [SerializeField] PairConfigSO pairConfigData;
        [SerializeField] Dictionary<ThemeCategory, CardCategoryDataSO> cardCategoryDataDic;
        [SerializeField] Dictionary<CardImgType, Sprite> cardImgDic;

        #region Header 
        [Space(10)]
        [Header("Gameplay Setting")]
        #endregion
        [SerializeField] GameplayProperty gameplayProperty;

        public PairConfigSO PairConfigData { get => pairConfigData; }
        public Dictionary<ThemeCategory, CardCategoryDataSO> CardCategoryDataDic { get => cardCategoryDataDic; }
        public Dictionary<CardImgType, Sprite> CardImgDic { get => cardImgDic; }
        public GameplayProperty GameplayProperty { get => gameplayProperty; }

        public void Init()
        {
            foreach (CardCategoryDataSO cardCategoryData in cardCategoryDataDic.Values)
            {
                cardCategoryData.SetObjValue();
            }
        }
    }
}