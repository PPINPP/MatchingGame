using MatchingGame.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MatchingGame.Gameplay
{
    [Serializable]
    public class GamplayLayoutSetting
    {
        public CategoryTheme categoryTheme;
        public PairType targetPairType;
        public GameDifficult gameDifficult;
        public GameLayout gameLayout;
    }

    public class SettingGameplay : MonoInstance<SettingGameplay>
    {
        [Header("Gamplay Setting")]
        [SerializeField] private CategoryTheme categoryTheme;
        [SerializeField] private PairType targetPairType;
        [SerializeField] private GameDifficult gameDifficult;
        [SerializeField] private GameLayout gameLayout;

        public Card cardPrefab;

        [Space(10)]
        [Header("UI Elements")]
        public GridLayoutGroup gridLayout;
        public GameObject randomLayout;

        public CategoryTheme CategoryTheme {  get { return categoryTheme; } }
        public PairType TargetPairType { get {  return targetPairType; } }
        public GameDifficult GameDifficult { get {  return gameDifficult; } }
        public GameLayout GameLayout { get {  return gameLayout; } }


        public void SetGameplaySetting(GamplayLayoutSetting setting)
        {
            categoryTheme = setting.categoryTheme;
            targetPairType = setting.targetPairType;
            gameDifficult = setting.gameDifficult;
            gameLayout = setting.gameLayout;
        }
    }
}