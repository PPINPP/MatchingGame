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
        public List<SequenceDetail> sequences = new List<SequenceDetail>();
    }

    [Serializable]
    public class SequenceDetail
    {
        [EnableIf("@!this.isMinigame && !this.isSmileyOMeter")]
        public bool isGamePlay;

        [HideIf(nameof(isGamePlay), true)]
        [EnableIf("@!this.isSmileyOMeter")]
        public bool isMinigame;

        [HideIf(nameof(isGamePlay), true)]
        [EnableIf("@!this.isMinigame")]
        public bool isSmileyOMeter;

        [ShowIf(nameof(isGamePlay), true)]
        public string stageID;

        [ShowIf(nameof(isGamePlay), true)]
        [SerializeField] GameplaySequenceSetting gameplay;

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