using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchingGame.Gameplay
{
    [CreateAssetMenu(fileName = "RandomPatternTranformData", menuName = "ScriptableObjects/RandomPatternTranformScriptable", order = 3)]
    public class RandomPatternTranformSO : SerializedScriptableObject
    {
        [SerializeField] Dictionary<PairType,List<SetCardTranform>> randomPatternData = new Dictionary<PairType, List<SetCardTranform>>();

        public SetCardTranform GetRandomSetCard(PairType pairType)
        {
            int ranNum = UnityEngine.Random.Range(0, randomPatternData[pairType].Count);
            return randomPatternData[pairType][ranNum];
        }

#if UNITY_EDITOR
        [Space(20)]
        [Header("SetupFile")]
        [SerializeField] PairType randomPatternType;
        [SerializeField] List<RectTransform> rectList;

        [Button]
        public void CreateData()
        {
            Dictionary<int, TranformDetail> tranforms = new Dictionary<int, TranformDetail>();
            for (int i = 0; i < rectList.Count; i++)
            {
                var rect = rectList[i];
                TranformDetail detail = new TranformDetail();
                detail.position = rect.localPosition;
                detail.rotation = rect.localRotation.eulerAngles;
                tranforms.Add(i, detail);
            }

            SetCardTranform cardSet = new SetCardTranform();
            cardSet.tranformCardInSet = tranforms;


            if (randomPatternData.ContainsKey(randomPatternType))
            {
                cardSet.setNumber = randomPatternData[randomPatternType].Count;
                randomPatternData[randomPatternType].Add(cardSet);
            }
            else
            {
                List<SetCardTranform> cards = new List<SetCardTranform>() { cardSet };
                randomPatternData.Add(randomPatternType, cards);
            }
        }

        [Button]
        public void ClearList()
        {
            rectList.Clear();
        }
#endif
    }

    [Serializable] 
    public class SetCardTranform
    {
        public int setNumber;
        public Dictionary<int,TranformDetail> tranformCardInSet;
    }

    [Serializable]
    public class TranformDetail
    {
        public Vector3 position;
        public Vector3 rotation;
    }
}