using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MatchingGame.Gameplay
{

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DifficultyScriptable", order = 2)]
    public class CardCategoryDataSO : SerializedScriptableObject
    {
        [Header("Load Img Config")]
        [SerializeField] string category;
        [SerializeField] string key;
        [SerializeField] int number;

        [Space(5)]
        [Header("Card Data Config")]
        public Dictionary<string, CardDataConfig> cardDataConfigDict = new Dictionary<string, CardDataConfig>();
        [HideInInspector]
        public List<string> easyObjID = new List<string>();
        [HideInInspector]
        public List<string> normalObjID = new List<string>();
        [HideInInspector]
        public List<string> hardObjID = new List<string>();
        [HideInInspector]
        public List<string> advanceObjID = new List<string>();

#if UNITY_EDITOR
        [ContextMenu("Load All Image")]
        [Button]
        public void LoadAllImg()
        {
            for (int i = 1; i < number; i++)
            {
                CreateDic(i);
            }
        }

        [ContextMenu("Load  Image")]
        [Button]
        public void LoadImg()
        {
            CreateDic(number);
        }

        [ContextMenu("Clear Data")]
        [Button]
        public void ClearData()
        {
            easyObjID.Clear();
            normalObjID.Clear();
            hardObjID.Clear();
            advanceObjID.Clear();
            cardDataConfigDict.Clear();
        }

        private void CreateDic(int number)
        {
            var Config = new CardDataConfig();

            string easyPath = $"Assets/Texture/{category}/1_Easy/{key}_E_{number:0000}.png";
            var easySprite = (Sprite)AssetDatabase.LoadAssetAtPath(easyPath, typeof(Sprite));
            Config.easySprite = easySprite;

            string normalPath = $"Assets/Texture/{category}/2_Normal/{key}_N_{number:0000}.png";
            var normalSprite = (Sprite)AssetDatabase.LoadAssetAtPath(normalPath, typeof(Sprite));
            Config.normalSprite = normalSprite;

            string hardPath = $"Assets/Texture/{category}/3_Hard/{key}_H_{number:0000}.png";
            var hardSprite = (Sprite)AssetDatabase.LoadAssetAtPath(hardPath, typeof(Sprite));
            Config.hardSprite = hardSprite;

            string advancePath = $"Assets/Texture/{category}/4_Advance/{key}_A_{number:0000}.png";
            var advanceSprite = (Sprite)AssetDatabase.LoadAssetAtPath(advancePath, typeof(Sprite));
            Config.advanceSprite = advanceSprite;

            cardDataConfigDict.Add($"{key}_{number:0000}", Config);
        }

#endif
        public void SetObjValue()
        {
            foreach (KeyValuePair<string, CardDataConfig> obj in cardDataConfigDict)
            {
                if (obj.Value.easySprite != null)
                {
                    obj.Value.SetEasyValue(true);

                    if (!easyObjID.Contains(obj.Key))
                        easyObjID.Add(obj.Key);
                }
                else
                {
                    easyObjID.Remove(obj.Key);
                    obj.Value.SetEasyValue(false);
                }



                if (obj.Value.normalSprite != null)
                {
                    obj.Value.SetNormalValue(true);

                    if (!normalObjID.Contains(obj.Key))
                        normalObjID.Add(obj.Key);
                }
                else
                {
                    normalObjID.Remove(obj.Key);
                    obj.Value.SetNormalValue(false);
                }

                if (obj.Value.hardSprite != null)
                {
                    obj.Value.SetHardValue(true);

                    if (!hardObjID.Contains(obj.Key))
                        hardObjID.Add(obj.Key);
                }
                else
                {
                    hardObjID.Remove(obj.Key);
                    obj.Value.SetHardValue(false);
                }

                if (obj.Value.advanceSprite != null)
                {
                    obj.Value.SetAdvancedValue(true);

                    if (!advanceObjID.Contains(obj.Key))
                        advanceObjID.Add(obj.Key);
                }
                else
                {
                    advanceObjID.Remove(obj.Key);
                    obj.Value.SetAdvancedValue(false);
                }
            }
        }

        public List<string> GetObjByGameDiff(GameDifficult gameDifficult)
        {
            switch (gameDifficult)
            {
                case GameDifficult.EASY:
                    return easyObjID;
                case GameDifficult.NORMAL:
                    return normalObjID;
                case GameDifficult.HARD:
                    return hardObjID;
                case GameDifficult.ADVANCE:
                    return advanceObjID;
                default:
                    List<string> result = new List<string>();
                    return result;
            }
        }
    }

    [Serializable]
    public class CardDataConfig
    {
        public bool isEasy { get; private set; }
        public bool isNormal { get; private set; }
        public bool isHard { get; private set; }
        public bool isAdvanced { get; private set; }
        public Sprite easySprite;
        public Sprite normalSprite;
        public Sprite hardSprite;
        public Sprite advanceSprite;

        public void SetEasyValue(bool value)
        {
            isEasy = value;
        }

        public void SetNormalValue(bool value)
        {
            isNormal = value;
        }

        public void SetHardValue(bool value)
        {
            isHard = value;
        }

        public void SetAdvancedValue(bool value)
        {
            isAdvanced = value;
        }
    }
}