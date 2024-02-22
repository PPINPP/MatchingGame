using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MatchingGame.Gameplay
{

    public enum GameDifficult
    {
        EASY,
        NORMAL,
        HARD,
        ADVANCE
    }

    public enum GameLayout
    {
        GRID,
        RANDOM
    }

    public enum ThemeCategory
    {
        HOME,
        CLOTH,
        MARKET
    }

    public class GameManager : SingletonSerializedMonobehaviour<GameManager>
    {
        [Header("Gamplay Setting")]
        [SerializeField] ThemeCategory categoryTheme;
        [SerializeField] PairType targetPairType;
        [SerializeField] GameDifficult gameDifficult;
        [SerializeField] GameLayout gameLayout;

        [SerializeField] Card cardPrefab;

        [Space(10)]
        [Header("UI Elements")]
        [SerializeField] GridLayoutGroup gridLayout;
        [SerializeField] GameObject randomLayout;
        [SerializeField] TextMeshProUGUI matchText;

        private PairConfig pairConfig = new PairConfig();
        private int _targetPairMatchCount;
        private int _remainPairMatchCount;
        private Transform _targetCardParent;
        private List<Card> _selectedCard;

        private void Start()
        {
            GameplayResources.Instance.Init();

            StartGame();
        }

        private void StartGame()
        {
            pairConfig = GameplayResources.Instance.PairConfigData.pairConfigs.Find(x => targetPairType == x.pairType);
            _targetPairMatchCount = (int)pairConfig.pairType;
            _remainPairMatchCount = _targetPairMatchCount;
            matchText.text = $"Number of Matches : {_remainPairMatchCount}";
            SettingLayout();
            InitializeCards();
        }

        private void SettingLayout()
        {
            if (gameLayout == GameLayout.GRID)
            {
                gridLayout.cellSize = new Vector2(pairConfig.cellSize.x, pairConfig.cellSize.y);
                gridLayout.constraintCount = pairConfig.ConstraintRow;
                _targetCardParent = gridLayout.transform;
            }
            else if (gameLayout == GameLayout.RANDOM)
            {

            }
        }

        //[Button]
        //private void TestRandom()
        //{
        //    var randomedCard = Utils.GetRndCardFromTargetAmount(_targetPairMatchCount, gameDifficult, GameplayResources.Instance.CardCatedoryDataDic[categoryTypeKey]);
        //    randomedCard.ForEach(x => {
        //        print($"===== Check ====");
        //        print($"Name : {x.Key}");
        //        print($"Easy : {x.Value.isEasy}");
        //        print($"2. Easy : {x.Value.easySprite}");
        //        print($"Normal : {x.Value.isNormal}");
        //        print($"2. Normal : {x.Value.normalSprite}");
        //        print($"Hard : {x.Value.isHard}");
        //        print($"2. Hard : {x.Value.hardSprite}");
        //        print($"Advance : {x.Value.isAdvanced}");
        //        print($"2. Advance : {x.Value.advanceSprite}");
        //    });

        //    var cardPropList = Utils.CreateCardPair(gameDifficult, randomedCard);
        //    print("***********");
        //    print($"Prop Count : {cardPropList.Count}");

        //    foreach (var item in cardPropList)
        //    {
        //        print("====");
        //        print($"1.key : {item.key}");
        //        print($"2.sprite {item.sprite.name}");
        //    }
        //}

        private void InitializeCards()
        {
            var randomedCard = GameplayUtils.GetRndCardFromTargetAmount(_targetPairMatchCount, gameDifficult, GameplayResources.Instance.CardCategoryDataDic[categoryTheme]);
            var cardPropList = GameplayUtils.CreateCardPair(gameDifficult, randomedCard);
            cardPropList = new List<CardProperty>(GameplayUtils.ShuffleCard(cardPropList, pairConfig.roundShuffle));

            foreach (var cardProp in cardPropList)
            {
                Instantiate(cardPrefab, _targetCardParent).Init(cardProp);
            }
        }


    }
}