using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace MatchingGame.Gameplay
{
    [CreateAssetMenu(fileName = "GameplaySequence", menuName = "ScriptableObjects/GameplaySequenceScriptable", order = 3)]
    public class GameplaySequenceSO : SerializedScriptableObject
    {
        [Title("Sequence Details")]
        public List<SequenceDetail> sequences = new List<SequenceDetail>();

#if UNITY_EDITOR
        [Title("Methods")]
        [Button]
        void CreateStageID()
        {
            int count = 1;
            foreach (var item in sequences)
            {
                if (item.isGamePlay && !item.GetGameplaySequenceSetting().isTutorial)
                {
                    item.stageID = 
                        $"{count:00}_{item.GetGameplaySequenceSetting().categoryTheme.ToString()}_{item.GetGameplaySequenceSetting().pairType.ToString()}_{item.GetGameplaySequenceSetting().GameDifficult.ToString()}_{item.GetGameplaySequenceSetting().layout.ToString()}";
                    count++;
                }
                    
            }
        }
#endif
    }

    [Serializable]
    public class SequenceDetail
    {
        [EnableIf("@!this.isMinigame && !this.isSmileyOMeter && !this.isDailyFeeling")]
        public bool isGamePlay;

        [HideIf(nameof(isGamePlay), true)]
        [EnableIf("@!this.isSmileyOMeter && !this.isDailyFeeling")]
        public bool isMinigame;

        [HideIf(nameof(isGamePlay), true)]
        [EnableIf("@!this.isMinigame && !this.isDailyFeeling")]
        public bool isSmileyOMeter;

        [HideIf(nameof(isGamePlay), true)]
        [EnableIf("@!this.isMinigame && !this.isSmileyOMeter")]
        public bool isDailyFeeling;

        [ShowIf(nameof(isGamePlay), true)]
        public string stageID;

        [ShowIf(nameof(isGamePlay), true)]
        [SerializeField] public GameplaySequenceSetting gameplay;

        public GameplaySequenceSetting GetGameplaySequenceSetting()
        {
            return gameplay;
        }

        //[ShowIf(nameof(isGamePlay),true)]
        //public bool isTutorial;
        //[ShowIf(nameof(isGamePlay), true)]
        //public CategoryTheme categoryTheme;
        //[ShowIf(nameof(isGamePlay), true)]
        //public PairType pairType;
        //[ShowIf(nameof(isGamePlay), true)]
        //public GameDifficult GameDifficult;
        //[ShowIf(nameof(isGamePlay), true)]
        //public GameLayout layout;
    }

    [Serializable]
    public class GameplaySequenceSetting
    {
        [Header("Tutorial Setting")]
        [HideIf(nameof(isForceCardID), true)]
        public bool isTutorial;

        [Header("Gameplay Setting")]
        [HideIf(nameof(isTutorial), true)]
        public bool isForceCardID;

        [ShowIf(nameof(isShowCardList), true)]
        public List<string> cardIDList;


        [Header("Layout Setting")]
        public CategoryTheme categoryTheme;
        public PairType pairType;
        public GameDifficult GameDifficult;
        public GameLayout layout;

        private bool isShowCardList()
        {
            return isForceCardID || isTutorial;
        }
    }
}